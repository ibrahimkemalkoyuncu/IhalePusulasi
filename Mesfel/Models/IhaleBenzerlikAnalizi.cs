using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class IhaleBenzerlikAnalizi
    {
        public Ihale Ihale { get; set; }
        public IhaleAnaliz Analiz { get; set; }

        [Display(Name = "Benzerlik Oranı")]
        [Range(0, 1)]
        public decimal BenzerlikOrani { get; set; }

        [Display(Name = "Ortalama Teklif Farkı")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrtalamaTeklifFarki { get; set; }

        [Display(Name = "Kalem Benzerlik Puanı")]
        public int KalemBenzerlikPuan { get; set; }

        [Display(Name = "Kategori Benzerlik Puanı")]
        public int KategoriBenzerlikPuan { get; set; }

        [Display(Name = "Teknik Şartname Benzerliği")]
        public bool TeknikSartnameBenzerligi { get; set; }
    }
}