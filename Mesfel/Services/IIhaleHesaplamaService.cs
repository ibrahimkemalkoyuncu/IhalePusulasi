using Mesfel.Sabitler;

namespace Mesfel.Services
{
    public interface IIhaleHesaplamaService
    {
        Task<IhaleAnalizSonucu> IhaleAnalizYapAsync(int ihaleId);
        Task<OptimalTeklifSonucu> OptimalTeklifHesaplaAsync(int ihaleId, decimal hedefKarOrani);
        Task<List<TeklifKarsilastirma>> TeklifKarsilastirmaYapAsync(int ihaleId);
        Task<GecimiTeminatHesapla> GecimiTeminatHesaplaAsync(decimal yaklasikMaliyet, IhaleTuru ihaleTuru);
        Task<KesinTeminatHesapla> KesinTeminatHesaplaAsync(decimal teklifTutari, double teminatOrani);
        Task<List<IstatistikVeri>> IhaleIstatistikleriGetirAsync();
    }
}
