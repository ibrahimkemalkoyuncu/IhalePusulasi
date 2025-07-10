using Mesfel.Models;
using Mesfel.Services;
using Mesfel.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Mesfel.Controllers
{
    public class IhaleController : Controller
    {
        private readonly IIhaleService _ihaleService;
        private readonly ILogger<IhaleController> _logger;

        public IhaleController(
            IIhaleService ihaleService,
            ILogger<IhaleController> logger)
        {
            _ihaleService = ihaleService;
            _logger = logger;
        }

        // Tüm ihaleleri listele (GET)
        public async Task<IActionResult> Index()
        {
            try
            {
                var ihaleler = await _ihaleService.GetIhalelerAsync();
                return View(ihaleler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler listelenirken hata oluştu.");
                return RedirectToAction("Error", "Home");
            }
        }

        // İhale detayını göster (GET)
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var ihale = await _ihaleService.GetByIdAsync(id);
                if (ihale == null)
                {
                    return NotFound();
                }
                return View(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale detayı görüntülenirken hata oluştu. ID: {IhaleId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // Yeni ihale formu (GET)
        public IActionResult Create()
        {
            return View(new Ihale
            {
                IhaleBaslangicTarihi = DateTime.Now,
                IhaleBitisTarihi = DateTime.Now.AddDays(7),
                IhaleDurumu = "Aktif"
            });
        }

        // Yeni ihale oluştur (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ihale ihale)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _ihaleService.CreateAsync(ihale);
                    return RedirectToAction(nameof(Index));
                }
                return View(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale oluşturulurken hata oluştu.");
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
                return View(ihale);
            }
        }

        // İhale düzenleme formu (GET)
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ihale = await _ihaleService.GetByIdAsync(id);
                if (ihale == null)
                {
                    return NotFound();
                }
                return View(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale düzenleme formu yüklenirken hata oluştu. ID: {IhaleId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // İhaleyi güncelle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ihale ihale)
        {
            if (id != ihale.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _ihaleService.UpdateAsync(ihale);
                    return RedirectToAction(nameof(Index));
                }
                return View(ihale);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "İhale güncellenirken çakışma hatası. ID: {IhaleId}", id);
                ModelState.AddModelError("", "Kayıt başka bir kullanıcı tarafından değiştirildi.");
                return View(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale güncellenirken hata oluştu. ID: {IhaleId}", id);
                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
                return View(ihale);
            }
        }

        // İhale silme onay formu (GET)
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ihale = await _ihaleService.GetByIdAsync(id);
                if (ihale == null)
                {
                    return NotFound();
                }
                return View(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silme onay formu yüklenirken hata oluştu. ID: {IhaleId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // İhaleyi sil (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _ihaleService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silinirken hata oluştu. ID: {IhaleId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // Duruma göre filtreleme
        public async Task<IActionResult> FilterByStatus(IhaleDurumu durum)
        {
            try
            {
                var ihaleler = await _ihaleService.GetByDurumAsync(durum);
                return View("Index", ihaleler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler duruma göre filtrelenirken hata oluştu. Durum: {Durum}", durum);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}