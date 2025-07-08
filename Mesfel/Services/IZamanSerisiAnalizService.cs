using Mesfel.Models;
using Mesfel.Sabitler;

namespace Mesfel.Services
{
    public interface IZamanSerisiAnalizService
    {
        Task<TeklifTrendAnalizi> TeklifTrendleriniAnalizEtAsync(int ihaleId);
        Task<List<IhaleTrendAnalizi>> IhaleTrendleriniAnalizEtAsync(IhaleTuru? ihaleTuru = null);
    }
}
