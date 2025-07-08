using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    // İhale Detay Model
    public class IhaleDetay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IhaleId { get; set; }

        [Required]
        [StringLength(200)]
        public string KalemAdi { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }

        [Required]
        public int Miktar { get; set; }

        [Required]
        [StringLength(50)]
        public string Birim { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar { get; set; }

        [StringLength(500)]
        public string Aciklama { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("IhaleId")]
        public virtual Ihale Ihale { get; set; }
    }

    // İhale Teklif Model
    public class IhaleTeklif
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IhaleId { get; set; }

        [Required]
        [StringLength(200)]
        public string FirmaAdi { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TeklifTutari { get; set; }

        [StringLength(500)]
        public string TeklifAciklamasi { get; set; }

        [Required]
        public DateTime TeklifTarihi { get; set; }

        [StringLength(50)]
        public string TeklifDurumu { get; set; } = "Beklemede";

        [StringLength(200)]
        public string IletisimBilgileri { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("IhaleId")]
        public virtual Ihale Ihale { get; set; }
    }

    // İhale Kategori Model
    public class IhaleKategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IhaleId { get; set; }

        [Required]
        public int KategoriId { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("IhaleId")]
        public virtual Ihale Ihale { get; set; }

        [ForeignKey("KategoriId")]
        public virtual Kategori Kategori { get; set; }
    }

    // Kategori Model
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string KategoriAdi { get; set; }

        [StringLength(500)]
        public string Aciklama { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<IhaleKategori> IhaleKategorileri { get; set; }
    }

    // Kullanıcı Model (Eğer yoksa)
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string KullaniciAdi { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [StringLength(15)]
        public string Telefon { get; set; }

        [StringLength(50)]
        public string Rol { get; set; } = "Kullanici";

        public bool AktifMi { get; set; } = true;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public DateTime? SonGirisTarihi { get; set; }
    }
}