using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    // İhale Kalemi Tablosu
    public class IhaleKalemi
    {
        [Key]
        public int IhaleKalemiId { get; set; }

        [Required]
        public int IhaleId { get; set; }

        [Required(ErrorMessage = "Kalem adı zorunludur")]
        [Display(Name = "Kalem Adı")]
        public string KalemAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birim zorunludur")]
        [Display(Name = "Birim")]
        public string Birim { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miktar zorunludur")]
        [Display(Name = "Miktar")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Miktar { get; set; }

        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Display(Name = "Birim Fiyat (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }

        [Display(Name = "Toplam Fiyat (TL)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamFiyat => Miktar * BirimFiyat;

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        // Navigation Properties
        public virtual Ihale Ihale { get; set; } = null!;
    }
}
