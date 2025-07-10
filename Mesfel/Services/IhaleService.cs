using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mesfel.Services
{
    public class IhaleService : IIhaleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleService> _logger;

        public IhaleService(ApplicationDbContext context, ILogger<IhaleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Tüm ihaleleri getir
        public async Task<IEnumerable<Ihale>> GetAllAsync()
        {
            return await _context.Ihaleler
                .Include(i => i.IhaleDetaylari)
                .Include(i => i.IhaleTeklifleri)
                .Include(i => i.IhaleKategorileri)
                .ThenInclude(ik => ik.Kategori)
                .OrderByDescending(i => i.KayitTarihi)
                .ToListAsync();
        }

        // ID'ye göre ihale getir
        public async Task<Ihale> GetByIdAsync(int id)
        {
            return await _context.Ihaleler
                .Include(i => i.IhaleDetaylari)
                .Include(i => i.IhaleTeklifleri)
                .Include(i => i.IhaleKategorileri)
                .ThenInclude(ik => ik.Kategori)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Yeni ihale ekle
        public async Task<Ihale> CreateAsync(Ihale ihale)
        {
            try
            {
                ihale.KayitTarihi = DateTime.Now;
                _context.Ihaleler.Add(ihale);
                await _context.SaveChangesAsync();
                return ihale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale eklenirken hata oluştu.");
                throw;
            }
        }

        // İhaleyi güncelle
        public async Task<Ihale> UpdateAsync(Ihale ihale)
        {
            try
            {
                ihale.GuncellemeTarihi = DateTime.Now;
                _context.Ihaleler.Update(ihale);
                await _context.SaveChangesAsync();
                return ihale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale güncellenirken hata oluştu. ID: {IhaleId}", ihale.Id);
                throw;
            }
        }

        // İhaleyi sil
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var ihale = await _context.Ihaleler.FindAsync(id);
                if (ihale == null) return false;

                _context.Ihaleler.Remove(ihale);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silinirken hata oluştu. ID: {IhaleId}", id);
                throw;
            }
        }

        // Duruma göre ihaleleri getir
        public async Task<IEnumerable<Ihale>> GetByDurumAsync(IhaleDurumu durum)
        {
            return await _context.Ihaleler
                .Where(i => i.IhaleDurumu == durum.ToString())
                .OrderByDescending(i => i.KayitTarihi)
                .ToListAsync();
        }

        // Arama yap
        public async Task<IEnumerable<Ihale>> SearchAsync(string searchTerm)
        {
            return await _context.Ihaleler
                .Where(i => i.IhaleAdi.Contains(searchTerm) ||
                           i.IhaleKurumu.Contains(searchTerm) ||
                           i.IhaleNumarasi.Contains(searchTerm))
                .ToListAsync();
        }

        // İhale numarası kontrolü
        public async Task<bool> IhaleNumarasiExistsAsync(string ihaleNumarasi, int? excludeId = null)
        {
            return await _context.Ihaleler
                .AnyAsync(i => i.IhaleNumarasi == ihaleNumarasi &&
                             (excludeId == null || i.Id != excludeId));
        }

        // GetIhalelerAsync (Önceki örnekte eklenmişti)
        public async Task<IEnumerable<Ihale>> GetIhalelerAsync()
        {
            return await _context.Ihaleler
                .Include(i => i.IhaleDetaylari)
                .Include(i => i.IhaleTeklifleri)
                .Include(i => i.IhaleKategorileri)
                .ThenInclude(ik => ik.Kategori)
                .OrderByDescending(i => i.KayitTarihi)
                .ToListAsync();
        }
    }
}