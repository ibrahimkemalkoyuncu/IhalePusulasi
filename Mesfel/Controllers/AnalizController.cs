using Mesfel.Models;
using Mesfel.Utilities;
using Mesfel.Services;
using Mesfel.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Mesfel.Controllers
{
    public class AnalizController : Controller
    {
        private readonly IIhaleKarsilastirmaService _karsilastirmaService;
        private readonly IZamanSerisiAnalizService _zamanSerisiService;
        private readonly IIhaleService _ihaleService;

        public AnalizController(
            IIhaleKarsilastirmaService karsilastirmaService,
            IZamanSerisiAnalizService zamanSerisiService,
            IIhaleService ihaleService)
        {
            _karsilastirmaService = karsilastirmaService;
            _zamanSerisiService = zamanSerisiService;
            _ihaleService = ihaleService;
        }

        [HttpGet]
        public async Task<IActionResult> Karsilastirma(int? ihaleId1, int? ihaleId2)
        {
            var model = new KarsilastirmaViewModel
            {
                TumIhaleler = await _ihaleService.GetAllAsync()
            };

            if (ihaleId1.HasValue && ihaleId2.HasValue)
            {
                model.Sonuc = await _karsilastirmaService.KarsilastirAsync(ihaleId1.Value, ihaleId2.Value);
                model.BenzerIhaleler1 = await _karsilastirmaService.BenzerIhaleleriBulAsync(ihaleId1.Value);
                model.BenzerIhaleler2 = await _karsilastirmaService.BenzerIhaleleriBulAsync(ihaleId2.Value);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TrendAnalizi(int? ihaleId, IhaleTuru? ihaleTuru)
        {
            var model = new TrendAnalizViewModel
            {
                TumIhaleler = await _ihaleService.GetAllAsync(),
                SeciliIhaleTuru = ihaleTuru
            };

            if (ihaleId.HasValue)
            {
                model.TeklifTrendleri = await _zamanSerisiService.TeklifTrendleriniAnalizEtAsync(ihaleId.Value);
            }

            model.IhaleTrendleri = await _zamanSerisiService.IhaleTrendleriniAnalizEtAsync(ihaleTuru);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BenzerIhaleAnalizi(int ihaleId)
        {
            var benzerIhaleler = await _karsilastirmaService.BenzerIhaleleriBulAsync(ihaleId);
            var ihale = await _ihaleService.GetByIdAsync(ihaleId);

            var model = new BenzerIhaleAnalizViewModel
            {
                HedefIhale = ihale,
                BenzerIhaleler = benzerIhaleler
            };

            return View(model);
        }
    }

    

    

   
}
