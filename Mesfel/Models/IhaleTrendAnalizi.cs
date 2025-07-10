using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class IhaleTrendAnalizi
    {
        [Display(Name = "Yıl")]
        public int Yil { get; set; }

        [Display(Name = "Ay")]
        public int Ay { get; set; }

        [Display(Name = "İhale Sayısı")]
        public int IhaleSayisi { get; set; }

        [Display(Name = "Ortalama Teklif Sayısı")]
        public double OrtalamaTeklifSayisi { get; set; }

        [Display(Name = "Ortalama Teklif Tutarı")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrtalamaTeklifTutari { get; set; }

        [Display(Name = "Ortalama Rekabet Seviyesi")]
        public double OrtalamaRekabetSeviyesi { get; set; }

        [Display(Name = "En Çok İhale Açan Kurum")]
        public string EnCokIhaleAcankurum { get; set; }

        [Display(Name = "En Düşük Teklif Oranı")]
        public double EnDusukTeklifOrani { get; set; }

        [Display(Name = "Tasarruf Oranı")]
        public double TasarrufOrani { get; set; }

        [Display(Name = "Trend Yönü")]
        public string TrendYonu { get; set; }
    }
}