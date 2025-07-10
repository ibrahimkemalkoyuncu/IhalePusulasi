using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class IhaleKategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Ihale")]
        public int IhaleId { get; set; }

        [Required]
        [ForeignKey("Kategori")]
        public int KategoriId { get; set; }

        [Display(Name = "Ana Kategori Mi?")]
        public bool AnaKategori { get; set; } = false;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Ihale Ihale { get; set; }
        public virtual Kategori Kategori { get; set; }
    }
}