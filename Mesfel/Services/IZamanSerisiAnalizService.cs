using Mesfel.Models;
using Mesfel.Utilities;

namespace Mesfel.Services
{
    public interface IZamanSerisiAnalizService
    {
        Task<TeklifTrendAnalizi> TeklifTrendleriniAnalizEtAsync(int ihaleId);
        Task<List<IhaleTrendAnalizi>> IhaleTrendleriniAnalizEtAsync(IhaleTuru? ihaleTuru = null);
    }
}
