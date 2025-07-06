using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mesfel.Controllers
{
    public class IhaleController : Controller
    {
        private readonly IIhaleService _ihaleService;
        private readonly ITeklifService _teklifService;
        private readonly IAnalizService _analizService;
        private readonly IRiskAnalizService _riskAnalizService;

        public IhaleController(
            IIhaleService ihaleService,
            ITeklifService teklifService,
            IAnalizService analizService,
            IRiskAnalizService riskAnalizService)
        {
            _ihaleService = ihaleService;
            _teklifService = teklifService;
            _analizService = analizService;
            _riskAnalizService = riskAnalizService;
        }

        [HttpGet]
        public async Task<IActionResult> Analiz(int id)
        {
            var ihale = await _ihaleService.GetByIdAsync(id);
            if (ihale == null)
            {
                return NotFound();
            }

            var teklifler = await _teklifService.GetByIhaleIdAsync(id);
            if (!teklifler.Any())
            {
                return RedirectToAction("Details", new { id, message = "Analiz için en az bir teklif gereklidir" });
            }

            var analiz = await _analizService.AnalizYapAsync(id);
            var strateji = await _analizService.StratejiOnerAsync(id);

            var model = new IhaleAnalizViewModel
            {
                Ihale = ihale,
                Analiz = analiz,
                Strateji = strateji,
                Teklifler = teklifler.OrderBy(t => t.TeklifTutari).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RiskAnalizi(int ihaleId, decimal teklifTutari)
        {
            var ihale = await _ihaleService.GetByIdAsync(ihaleId);
            if (ihale == null)
            {
                return NotFound();
            }

            var analiz = await _analizService.AnalizYapAsync(ihaleId);
            var riskAnaliz = _riskAnalizService.RiskAnaliziYap(ihale, analiz, teklifTutari);

            return PartialView("_RiskAnalizSonucu", riskAnaliz);
        }

        [HttpGet]
        public async Task<IActionResult> MonteCarloSimulasyonu(int ihaleId)
        {
            var ihale = await _ihaleService.GetByIdAsync(ihaleId);
            if (ihale == null)
            {
                return NotFound();
            }

            var analiz = await _analizService.AnalizYapAsync(ihaleId);
            var senaryolar = _riskAnalizService.MonteCarloSimulasyonu(ihale, analiz, 100);

            var model = new MonteCarloViewModel
            {
                Ihale = ihale,
                Analiz = analiz,
                Senaryolar = senaryolar,
                BasariliSenaryolar = senaryolar.Count(s => s.KazanmaDurumu && !s.AsiriDusuk),
                BasarisizSenaryolar = senaryolar.Count(s => !s.KazanmaDurumu || s.AsiriDusuk)
            };

            return View(model);
        }
    }

    public class IhaleAnalizViewModel
    {
        public Ihale Ihale { get; set; }
        public IhaleAnaliz Analiz { get; set; }
        public TeklifStratejisi Strateji { get; set; }
        public List<Teklif> Teklifler { get; set; }
    }

    public class MonteCarloViewModel
    {
        public Ihale Ihale { get; set; }
        public IhaleAnaliz Analiz { get; set; }
        public List<RiskSenaryo> Senaryolar { get; set; }
        public int BasariliSenaryolar { get; set; }
        public int BasarisizSenaryolar { get; set; }
    }
}