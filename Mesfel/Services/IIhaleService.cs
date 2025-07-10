using Mesfel.Models;
using Mesfel.Utilities;

namespace Mesfel.Services
{
    public interface IIhaleService
    {
        Task<IEnumerable<Ihale>> GetAllAsync();
        Task<Ihale> GetByIdAsync(int id);
        Task<Ihale> CreateAsync(Ihale ihale);
        Task<Ihale> UpdateAsync(Ihale ihale);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Ihale>> GetByDurumAsync(IhaleDurumu durum);
        Task<IEnumerable<Ihale>> SearchAsync(string searchTerm);
        Task<bool> IhaleNumarasiExistsAsync(string ihaleNumarasi, int? excludeId = null);

        Task<IEnumerable<Ihale>> GetIhalelerAsync();
    }
}
