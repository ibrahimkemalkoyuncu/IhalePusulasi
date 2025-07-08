using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Services;

namespace Mesfel.Controllers
{
    public class IhaleController : Controller
    {
        private readonly MesfelDbContext _context;
        private readonly IIhaleService _ihaleService;

        public IhaleController(MesfelDbContext context, IIhaleService ihaleService)
        {
            _context = context;
            _ihaleService = ihaleService;
        }

        // GET: Ihale
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            var ihaleler = await _ihaleService.GetIhalelerAsync(searchTerm, page, pageSize);
            return View(ihaleler);
        }

        // GET: Ihale/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ihale = await _context.Ihaleler
                .Include(i => i.IhaleDetaylari)
                .Include(i => i.IhaleTeklifleri)
                .Include(i => i.IhaleKategorileri)
                    .ThenInclude(ik => ik.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ihale == null)
            {
                return NotFound();
            }

            return View(ihale);
        }

        // GET: Ihale/Create
        public IActionResult Create()
        {
            ViewBag.Kategoriler = _context.Kategoriler.Where(k => k.AktifMi).ToList();
            return View();
        }

        // POST: Ihale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IhaleAdi,IhaleKurumu,KesifBedeli,IhaleBaslangicTarihi,IhaleBitisTarihi,IhaleTuru,Aciklama,IhaleLinki,IhaleNumarasi,IletisimBilgileri,IhaleUsulu")] Ihale ihale, int[] selectedKategoriler)
        {
            if (ModelState.IsValid)
            {
                ihale.KayitTarihi = DateTime.Now;
                ihale.KaydedenKullanici = User.Identity.Name ?? "Sistem";
                ihale.IhaleDurumu = "Aktif";

                _context.Add(ihale);
                await _context.SaveChangesAsync();

                // Kategori ilişkileri ekle
                if (selectedKategoriler != null && selectedKategoriler.Length > 0)
                {
                    foreach (var kategoriId in selectedKategoriler)
                    {
                        var ihaleKategori = new IhaleKategori
                        {
                            IhaleId = ihale.Id,
                            KategoriId = kategoriId,
                            KayitTarihi = DateTime.Now
                        };
                        _context.IhaleKategorileri.Add(ihaleKategori);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "İhale başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Kategoriler = _context.Kategoriler.Where(k => k.AktifMi).ToList();
            return View(ihale);
        }

        // GET: Ihale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ihale = await _context.Ihaleler
                .Include(i => i.IhaleKategorileri)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ihale == null)
            {
                return NotFound();
            }

            ViewBag.Kategoriler = _context.Kategoriler.Where(k => k.AktifMi).ToList();
            ViewBag.SelectedKategoriler = ihale.IhaleKategorileri.Select(ik => ik.KategoriId).ToArray();

            return View(ihale);
        }

        // POST: Ihale/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IhaleAdi,IhaleKurumu,KesifBedeli,IhaleBaslangicTarihi,IhaleBitisTarihi,IhaleTuru,Aciklama,IhaleLinki,IhaleNumarasi,IletisimBilgileri,IhaleUsulu,IhaleDurumu")] Ihale ihale, int[] selectedKategoriler)
        {
            if (id != ihale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingIhale = await _context.Ihaleler
                        .Include(i => i.IhaleKategorileri)
                        .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingIhale == null)
                    {
                        return NotFound();
                    }

                    // Mevcut özellikleri güncelle
                    existingIhale.IhaleAdi = ihale.IhaleAdi;
                    existingIhale.IhaleKurumu = ihale.IhaleKurumu;
                    existingIhale.KesifBedeli = ihale.KesifBedeli;
                    existingIhale.IhaleBaslangicTarihi = ihale.IhaleBaslangicTarihi;
                    existingIhale.IhaleBitisTarihi = ihale.IhaleBitisTarihi;
                    existingIhale.IhaleTuru = ihale.IhaleTuru;
                    existingIhale.Aciklama = ihale.Aciklama;
                    existingIhale.IhaleLinki = ihale.IhaleLinki;
                    existingIhale.IhaleNumarasi = ihale.IhaleNumarasi;
                    existingIhale.IletisimBilgileri = ihale.IletisimBilgileri;
                    existingIhale.IhaleUsulu = ihale.IhaleUsulu;
                    existingIhale.IhaleDurumu = ihale.IhaleDurumu;
                    existingIhale.GuncellemeTarihi = DateTime.Now;
                    existingIhale.GuncelleyenKullanici = User.Identity.Name ?? "Sistem";

                    // Mevcut kategori ilişkilerini sil
                    _context.IhaleKategorileri.RemoveRange(existingIhale.IhaleKategorileri);

                    // Yeni kategori ilişkilerini ekle
                    if (selectedKategoriler != null && selectedKategoriler.Length > 0)
                    {
                        foreach (var kategoriId in selectedKategoriler)
                        {
                            var ihaleKategori = new IhaleKategori
                            {
                                IhaleId = existingIhale.Id,
                                KategoriId = kategoriId,
                                KayitTarihi = DateTime.Now
                            };
                            _context.IhaleKategorileri.Add(ihaleKategori);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "İhale başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IhaleExists(ihale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Kategoriler = _context.Kategoriler.Where(k => k.AktifMi).ToList();
            ViewBag.SelectedKategoriler = selectedKategoriler;
            return View(ihale);
        }

        // GET: Ihale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ihale = await _context.Ihaleler
                .Include(i => i.IhaleKategorileri)
                    .ThenInclude(ik => ik.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ihale == null)
            {
                return NotFound();
            }

            return View(ihale);
        }

        // POST: Ihale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);
            if (ihale != null)
            {
                _context.Ihaleler.Remove(ihale);
                await _context.SaveChangesAsync();
                TempData["Success"] = "İhale başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // API Methods
        [HttpGet]
        public async Task<IActionResult> GetIhaleDetaylari(int ihaleId)
        {
            var detaylar = await _context.IhaleDetaylari
                .Where(d => d.IhaleId == ihaleId)
                .ToListAsync();

            return Json(detaylar);
        }

        [HttpPost]
        public async Task<IActionResult> AddIhaleDetay([FromBody] IhaleDetay detay)
        {
            if (ModelState.IsValid)
            {
                detay.ToplamTutar = detay.BirimFiyat * detay.Miktar;
                detay.KayitTarihi = DateTime.Now;

                _context.IhaleDetaylari.Add(detay);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Detay başarıyla eklendi." });
            }

            return Json(new { success = false, message = "Geçersiz veri." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIhaleDurumu(int id, string durum)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);
            if (ihale != null)
            {
                ihale.IhaleDurumu = durum;
                ihale.GuncellemeTarihi = DateTime.Now;
                ihale.GuncelleyenKullanici = User.Identity.Name ?? "Sistem";

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Durum başarıyla güncellendi." });
            }

            return Json(new { success = false, message = "İhale bulunamadı." });
        }

        private bool IhaleExists(int id)
        {
            return _context.Ihaleler.Any(e => e.Id == id);
        }
    }
}