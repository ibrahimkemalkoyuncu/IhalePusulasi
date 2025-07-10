using System.ComponentModel.DataAnnotations;

namespace Mesfel.Models
{
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
}
