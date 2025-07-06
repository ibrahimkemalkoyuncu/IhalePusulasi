using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mesfel.Sabitler;  


namespace Mesfel.Models
{
    // Teklif Tablosu
    public class Teklif
    {
        [Key]
        public int TeklifId { get; set; }

        [Required]
        public int IhaleId { get; set; }

        [Required(ErrorMessage = "Firma adı zorunludur")]
        [Display(Name = "Firma Adı")]
        public string FirmaAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vergi numarası zorunludur")]
        [Display(Name = "Vergi Numarası")]
        public string VergiNumarasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teklif tutarı zorunludur")]
        [Display(Name = "Teklif Tutarı (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TeklifTutari { get; set; }

        [Display(Name = "KDV Tutarı (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal KdvTutari { get; set; }

        [Display(Name = "Genel Toplam (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GenelToplam => TeklifTutari + KdvTutari;

        [Display(Name = "Yaklaşık Maliyete Göre Oran (%)")]
        public double YaklasikMaliyetOrani { get; set; }

        [Display(Name = "Teklif Durumu")]
        public TeklifDurumu TeklifDurumu { get; set; } = TeklifDurumu.Verildi;

        [Display(Name = "Teklif Tarihi")]
        public DateTime TeklifTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        // Navigation Properties
        public virtual Ihale Ihale { get; set; } = null!;
    }
}
