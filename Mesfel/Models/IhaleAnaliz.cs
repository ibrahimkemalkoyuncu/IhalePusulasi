using Mesfel.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class IhaleAnaliz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "İhale ID")]
        public int IhaleId { get; set; }

        [Required]
        [Display(Name = "Analiz Tipi")]
        [StringLength(100)]
        public string AnalizTipi { get; set; } = string.Empty;

        [Display(Name = "Sonuç")]
        [StringLength(1000)]
        public string Sonuc { get; set; } = string.Empty;

        [Display(Name = "Analiz Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime AnalizTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Puan")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Puan { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(2000)]
        public string? Aciklama { get; set; }

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellenmeTarihi { get; set; }

        // İhale analiz özellikleri
        [Display(Name = "Toplam Teklif Sayısı")]
        public int ToplamTeklifSayisi { get; set; }

        [Display(Name = "Ortalama Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrtalamaTeklif { get; set; }

        [Display(Name = "En Düşük Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EnDusukTeklif { get; set; }

        [Display(Name = "En Yüksek Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EnYuksekTeklif { get; set; }

        [Display(Name = "Kazanan Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? KazananTeklif { get; set; }

        [Display(Name = "Rekabet Oranı")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? RekabetOrani { get; set; }

        [Display(Name = "Tasarruf Oranı")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? TasarrufOrani { get; set; }

        [Display(Name = "Standart Sapma")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StandartSapma { get; set; }

        [Display(Name = "Tahmin Edilen Kazanan Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TahminEdilenKazananTeklif { get; set; }


        [Display(Name = "Rekabet Seviyesi")]
        public RekabetSeviyesi RekabetSeviyesi { get; set; }


        // Navigation Property
        [ForeignKey("IhaleId")]
        public virtual Ihale Ihale { get; set; } = null!;


        public List<IhaleTeklif> IhaleTeklifler { get; set; } = new List<IhaleTeklif>();
    }
}