namespace Mesfel.Services
{
    public interface IIhaleKarsilastirmaService
    {
        Task<IhaleKarsilastirmaSonucu> KarsilastirAsync(int ihaleId1, int ihaleId2);
        Task<List<IhaleBenzerlikAnalizi>> BenzerIhaleleriBulAsync(int ihaleId);
    }
}
