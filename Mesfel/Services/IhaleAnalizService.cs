using Mesfel.Data;
using Mesfel.Models;
using Microsoft.EntityFrameworkCore;


// Services/IhaleAnalizService.cs
namespace Mesfel.Services
{
    public class IhaleAnalizService : IIhaleAnalizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleAnalizService> _logger;

        public IhaleAnalizService(ApplicationDbContext context, ILogger<IhaleAnalizService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IhaleAnaliz> AnalizYapAsync(int ihaleId)
        {
            // Analiz yapma işlemleri burada
            // Örnek basit implementasyon:
            var ihale = await _context.Ihaleler
                .Include(i => i.IhaleTeklifleri)
                .FirstOrDefaultAsync(i => i.Id == ihaleId);

            if (ihale == null) throw new ArgumentException("İhale bulunamadı");

            var analiz = new IhaleAnaliz
            {
                IhaleId = ihaleId,
                AnalizTarihi = DateTime.Now,
                ToplamTeklifSayisi = ihale.IhaleTeklifleri.Count,
                OrtalamaTeklif = ihale.IhaleTeklifleri.Average(t => t.TeklifTutari),
                // Diğer analiz sonuçları...
            };

            _context.IhaleAnalizleri.Add(analiz);
            await _context.SaveChangesAsync();

            return analiz;
        }
    }
}