using Mesfel.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class Ihale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string IhaleAdi { get; set; }

        [Required]
        [StringLength(200)]
        public string IhaleKurumu { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal KesifBedeli { get; set; }

        [Required]
        public DateTime IhaleBaslangicTarihi { get; set; }

        [Required]
        public DateTime IhaleBitisTarihi { get; set; }

        [Required]
        [StringLength(100)]
        public IhaleTuru IhaleTuru { get; set; }

        [StringLength(1000)]
        public string Aciklama { get; set; }

        [StringLength(500)]
        public string IhaleLinki { get; set; }

        [StringLength(100)]
        public string IhaleNumarasi { get; set; }

        [StringLength(200)]
        public string IletisimBilgileri { get; set; }

        [StringLength(100)]
        public string IhaleUsulu { get; set; }

        [StringLength(50)]
        public string IhaleDurumu { get; set; } = "Aktif";

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public DateTime? GuncellemeTarihi { get; set; }

        [StringLength(100)]
        public string KaydedenKullanici { get; set; }

        [StringLength(100)]
        public string GuncelleyenKullanici { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal YaklasikMaliyet { get; set; }  // Eksik olan property

        [Required]
        public DateTime TeklifSonTarihi { get; set; }  // Tekliflerin son verilebileceği tarih



        // Navigation properties


        public virtual ICollection<IhaleDetay> IhaleDetaylari { get; set; }
        public virtual ICollection<IhaleTeklif> IhaleTeklifleri { get; set; }
        public virtual ICollection<IhaleKategori> IhaleKategorileri { get; set; }

        // Navigation property for analysis
        public virtual ICollection<IhaleAnaliz> IhaleAnalizleri { get; set; } = new List<IhaleAnaliz>();

        // Navigation properties
        public virtual ICollection<IhaleKalemi> IhaleKalemleri { get; set; } = new List<IhaleKalemi>();

    }
}