using Mesfel.Utilities;
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
}