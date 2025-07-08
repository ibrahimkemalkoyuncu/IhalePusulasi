using Mesfel.Models;

namespace Mesfel.Services
{
    public interface ITeklifService
    {
        Task<IEnumerable<Teklif>> GetAllAsync();
        Task<Teklif> GetByIdAsync(int id);
        Task<Teklif> CreateAsync(Teklif teklif);
        Task<Teklif> UpdateAsync(Teklif teklif);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Teklif>> GetByIhaleIdAsync(int ihaleId);
        Task<Teklif> GetKazananTeklifAsync(int ihaleId);
        Task<bool> SetKazananTeklifAsync(int teklifId);
        Task<IEnumerable<Teklif>> GetAsiriDusukTekliflerAsync(int ihaleId);
        Task<decimal> CalculateTeklifYuzdesiAsync(int ihaleId, decimal teklifTutari);
    }
}
