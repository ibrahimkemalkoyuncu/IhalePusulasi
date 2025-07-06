using Mesfel.Models;
using Mesfel.Sabitler;

namespace Mesfel.Services
{
    public interface IKamuIhaleHesaplamaService
    {
        decimal AsiriDusukTeklifSiniriHesapla(Ihale ihale, IhaleAnaliz analiz);
        bool AsiriDusukTeklifMi(Ihale ihale, decimal teklifTutari, IhaleAnaliz analiz);
        decimal HesaplaAltSinir(IhaleTuru ihaleTuru, decimal kesifBedeli);
        decimal HesaplaUstSinir(IhaleTuru ihaleTuru, decimal kesifBedeli);
        decimal HesaplaOptimalTeklif(Ihale ihale, IhaleAnaliz analiz);
    }
}
