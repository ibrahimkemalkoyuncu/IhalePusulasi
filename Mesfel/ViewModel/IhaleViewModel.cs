using Mesfel.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Mesfel.ViewModel
{
    public class IhaleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İhale adı zorunludur")]
        [StringLength(500, ErrorMessage = "İhale adı en fazla 500 karakter olabilir")]
        [Display(Name = "İhale Adı")]
        public string IhaleAdi { get; set; }

        [Required(ErrorMessage = "Kurum adı zorunludur")]
        [StringLength(200, ErrorMessage = "Kurum adı en fazla 200 karakter olabilir")]
        [Display(Name = "Kurum Adı")]
        public string IhaleKurumu { get; set; }

        [Required(ErrorMessage = "Keşif bedeli zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Geçerli bir tutar girin")]
        [Display(Name = "Keşif Bedeli")]
        public decimal KesifBedeli { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime IhaleBaslangicTarihi { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur")]
        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime IhaleBitisTarihi { get; set; }

        [Required(ErrorMessage = "İhale türü zorunludur")]
        [Display(Name = "İhale Türü")]
        public IhaleTuru IhaleTuru { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL girin")]
        [Display(Name = "İhale Linki")]
        public string IhaleLinki { get; set; }

        [Display(Name = "İhale Numarası")]
        public string IhaleNumarasi { get; set; }

        [Display(Name = "İletişim Bilgileri")]
        public string IletisimBilgileri { get; set; }

        [Display(Name = "İhale Usulü")]
        public string IhaleUsulu { get; set; }

        [Display(Name = "Durum")]
        public string IhaleDurumu { get; set; } = "Aktif";

        // View-specific properties
        [Display(Name = "Toplam Teklif Sayısı")]
        public int ToplamTeklifSayisi { get; set; }

        [Display(Name = "Ortalama Teklif")]
        [DataType(DataType.Currency)]
        public decimal? OrtalamaTeklif { get; set; }

        [Display(Name = "Kalan Süre")]
        public string KalanSure
        {
            get
            {
                var kalan = IhaleBitisTarihi - DateTime.Now;
                return kalan.TotalDays > 0 ? $"{kalan.Days}g {kalan.Hours}sa {kalan.Minutes}d" : "Süre Doldu";
            }
        }

        // Navigation properties (simplified for view)
        public List<IhaleKalemViewModel> IhaleKalemleri { get; set; } = new List<IhaleKalemViewModel>();
        public List<IhaleTeklifViewModel> IhaleTeklifleri { get; set; } = new List<IhaleTeklifViewModel>();
        public List<string> Kategoriler { get; set; } = new List<string>();

        // Analysis summary
        public string AnalizOzeti { get; set; }
    }
}
