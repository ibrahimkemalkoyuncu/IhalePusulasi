using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mesfel.Sabitler;

namespace Mesfel.Models
{
    // İhale Ana Tablosu
    public class Ihale
    {
        [Key]
        public int IhaleId { get; set; }

        [Required(ErrorMessage = "İhale adı zorunludur")]
        [Display(Name = "İhale Adı")]
        public string IhaleAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "İhale kodu zorunludur")]
        [Display(Name = "İhale Kodu")]
        public string IhaleKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "İhale türü zorunludur")]
        [Display(Name = "İhale Türü")]
        public IhaleTuru IhaleTuru { get; set; }

        [Required(ErrorMessage = "İhale usulü zorunludur")]
        [Display(Name = "İhale Usulü")]
        public IhaleUsulu IhaleUsulu { get; set; }

        [Required(ErrorMessage = "İhale konusu zorunludur")]
        [Display(Name = "İhale Konusu")]
        public string IhaleKonusu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yaklaşık maliyet zorunludur")]
        [Display(Name = "Yaklaşık Maliyet (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal YaklasikMaliyet { get; set; }

        [Display(Name = "Geçici Teminat (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GeciciTeminat { get; set; }

        [Display(Name = "Kesin Teminat Oranı (%)")]
        public double KesinTeminatOrani { get; set; } = 6.0; // Varsayılan %6

        [Required(ErrorMessage = "İhale tarihi zorunludur")]
        [Display(Name = "İhale Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime IhaleTarihi { get; set; }

        [Required(ErrorMessage = "Teklif verme süresi zorunludur")]
        [Display(Name = "Teklif Verme Son Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime TeklifSonTarihi { get; set; }

        [Display(Name = "İhale Durumu")]
        public IhaleDurumu IhaleDurumu { get; set; } = IhaleDurumu.Planlanmakta;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellenmeTarihi { get; set; }

        // Navigation Properties
        public virtual ICollection<Teklif> Teklifler { get; set; } = new List<Teklif>();
        public virtual ICollection<IhaleKalemi> IhaleKalemleri { get; set; } = new List<IhaleKalemi>();
    }
}
