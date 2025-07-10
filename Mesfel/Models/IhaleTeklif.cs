
using Mesfel.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class IhaleTeklif
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Ihale")]
        public int IhaleId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Firma adı en fazla 200 karakter olabilir")]
        [Display(Name = "Firma Adı")]
        public string FirmaAdi { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Vergi no 10 veya 11 karakter olmalı")]
        [Display(Name = "Vergi No")]
        public string VergiNo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Teklif tutarı sıfırdan büyük olmalı")]
        [Display(Name = "Teklif Tutarı")]
        public decimal TeklifTutari { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "KDV Tutarı")]
        public decimal KdvTutari { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Genel Toplam")]
        public decimal GenelToplam => TeklifTutari + KdvTutari;

        [Display(Name = "Teklif Durumu")]
        public TeklifDurumu TeklifDurumu { get; set; } = TeklifDurumu.Verildi; // Doğru kullanım

        [Display(Name = "Teklif Tarihi")]
        public DateTime TeklifTarihi { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Aciklama { get; set; }

        [Display(Name = "Geçerli Teklif Mi?")]
        public bool GecerliTeklif { get; set; } = true;

        // Navigation property
        public virtual Ihale Ihale { get; set; }
    }

}