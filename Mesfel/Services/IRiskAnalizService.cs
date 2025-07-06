using Mesfel.Models;

namespace Mesfel.Services
{
    public interface IRiskAnalizService
    {
        RiskAnalizSonucu RiskAnaliziYap(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari);
        decimal AsiriDusukKalmaOlasiligi(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari);
        decimal UstundeKalmaOlasiligi(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari);
        List<RiskSenaryo> MonteCarloSimulasyonu(Ihale ihale, IhaleAnaliz analiz, int simulasyonSayisi = 1000);
    }
}
