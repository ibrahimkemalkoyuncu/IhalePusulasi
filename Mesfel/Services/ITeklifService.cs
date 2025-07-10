using Mesfel.Models;

namespace Mesfel.Services
{
    public interface ITeklifService
    {
        Task<IEnumerable<IhaleTeklif>> GetAllAsync();
        Task<IhaleTeklif> GetByIdAsync(int id);
        Task<IhaleTeklif> CreateAsync(IhaleTeklif teklif);
        Task<IhaleTeklif> UpdateAsync(IhaleTeklif teklif);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IhaleTeklif>> GetByIhaleIdAsync(int ihaleId);
        Task<IhaleTeklif> GetKazananTeklifAsync(int ihaleId);
        Task<bool> SetKazananTeklifAsync(int teklifId);
        Task<IEnumerable<IhaleTeklif>> GetAsiriDusukTekliflerAsync(int ihaleId);
        Task<decimal> CalculateTeklifYuzdesiAsync(int ihaleId, decimal teklifTutari);
    }
}
