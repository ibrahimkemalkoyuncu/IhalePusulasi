using System.ComponentModel.DataAnnotations;

namespace Mesfel.Models
{
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

        [Required]
        [StringLength(255)] // Hash'lenmiş şifre için yeterli uzunluk
        public string SifreHash { get; set; }

        [StringLength(50)]
        public string Rol { get; set; } = "Kullanici";

        public bool AktifMi { get; set; } = true;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public DateTime? SonGirisTarihi { get; set; }
    }
}
