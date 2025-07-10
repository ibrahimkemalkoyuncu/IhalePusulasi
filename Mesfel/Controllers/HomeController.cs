using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Utilities;
using Mesfel.Services;
using Mesfel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Mesfel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IIhaleHesaplamaService _ihaleHesaplamaService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IIhaleHesaplamaService ihaleHesaplamaService)
        {
            _logger = logger;
            _context = context;
            _ihaleHesaplamaService = ihaleHesaplamaService;
        }

        /// <summary>
        /// Ana sayfa - Ýhale listesi ve genel istatistikleri gösterir
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Son ihaleler
                var sonIhaleler = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .OrderByDescending(i => i.KayitTarihi)
                    .Take(5)
                    .ToListAsync();

                // Aktif ihaleler
                var aktifIhaleler = await _context.Ihaleler
                        .Where(i => i.IhaleDurumu == nameof(IhaleDurumu.TeklifAlma) || i.IhaleDurumu == nameof(IhaleDurumu.IlanEdildi))
                    .Include(i => i.IhaleTeklifleri)
                    .ToListAsync();

                // Genel istatistikler
                 var istatistikler = await _ihaleHesaplamaService.IhaleIstatistikleriGetirAsync();



                // Dashboard model oluþtur
                var dashboardModel = new DashboardViewModel
                {
                    SonIhaleler = sonIhaleler,
                    AktifIhaleler = aktifIhaleler,
                    Istatistikler = istatistikler.Select(s => new Mesfel.ViewModel.IstatistikVeri
                    {
                        Baslik = s.Baslik,
                        Deger = s.Deger
                        
                    }).ToList()
                };

                return View(dashboardModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana sayfa yüklenirken hata oluþtu");
                return View("Error");
            }
        }

        /// <summary>
        /// Ýhale detay sayfasý
        /// </summary>
        public async Task<IActionResult> IhaleDetay(int id)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Include(i => i.IhaleKalemleri)
                    .FirstOrDefaultAsync(i => i.Id  == id);

                if (ihale == null)
                {
                    return NotFound("Ýhale bulunamadý");
                }

                // Ýhale analizi yap
                var analizSonucu = await _ihaleHesaplamaService.IhaleAnalizYapAsync(id);

                // Teklif karþýlaþtýrmasý
                var teklifKarsilastirma = await _ihaleHesaplamaService.TeklifKarsilastirmaYapAsync(id);

                // Geçici teminat hesapla
                var geciciTeminat = await _ihaleHesaplamaService.GecimiTeminatHesaplaAsync(ihale.YaklasikMaliyet, ihale.IhaleTuru);

                var detayModel = new IhaleDetayViewModel
                {
                    Ihale = ihale,
                    AnalizSonucu = analizSonucu,
                    TeklifKarsilastirma = teklifKarsilastirma,
                    GeciciTeminat = geciciTeminat
                };

                return View(detayModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ýhale detay sayfasý yüklenirken hata oluþtu. Ýhale ID: {IhaleId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Optimal teklif hesaplama sayfasý
        /// </summary>
        public async Task<IActionResult> OptimalTeklif(int id)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (ihale == null)
                {
                    return NotFound("Ýhale bulunamadý");
                }

                var model = new OptimalTeklifViewModel
                {
                    Ihale = ihale,
                    HedefKarOrani = 15 // Varsayýlan %15 kar
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Optimal teklif sayfasý yüklenirken hata oluþtu. Ýhale ID: {IhaleId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Optimal teklif hesaplama POST iþlemi
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> OptimalTeklif(OptimalTeklifViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var ihale = await _context.Ihaleler
                        .Include(i => i.IhaleTeklifleri)
                        .FirstOrDefaultAsync(i => i.Id == model.Ihale.Id);
                    model.Ihale = ihale!;
                    return View(model);
                }

                // Optimal teklif hesapla
                var optimalSonuc = await _ihaleHesaplamaService.OptimalTeklifHesaplaAsync(model.Ihale.Id, model.HedefKarOrani);

                // Kesin teminat hesapla
                var kesinTeminat = await _ihaleHesaplamaService.KesinTeminatHesaplaAsync(optimalSonuc.OptimalTeklifTutari, 6.0);

                model.OptimalSonuc = optimalSonuc;
                model.KesinTeminat = kesinTeminat;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Optimal teklif hesaplanýrken hata oluþtu. Ýhale ID: {Id}", model.Ihale.Id);
                ModelState.AddModelError("", "Hesaplama sýrasýnda bir hata oluþtu. Lütfen tekrar deneyin.");
                return View(model);
            }
        }

        /// <summary>
        /// Teklif karþýlaþtýrma sayfasý
        /// </summary>
        public async Task<IActionResult> TeklifKarsilastirma(int id)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (ihale == null)
                {
                    return NotFound("Ýhale bulunamadý");
                }

                var karsilastirma = await _ihaleHesaplamaService.TeklifKarsilastirmaYapAsync(id);

                var model = new TeklifKarsilastirmaViewModel
                {
                    Ihale = ihale,
                    TeklifKarsilastirmalari = karsilastirma
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif karþýlaþtýrma sayfasý yüklenirken hata oluþtu. Ýhale ID: {IhaleId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Hakkýnda sayfasý
        /// </summary>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Ýletiþim sayfasý
        /// </summary>
        public IActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Hata sayfasý
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}