using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mesfel.Models
{
    public class TeklifTrendVeriNoktasi
    {
        [Display(Name = "Tarih")]
        public DateTime Tarih { get; set; }

        [Display(Name = "Teklif Sayısı")]
        public int TeklifSayisi { get; set; }

        [Display(Name = "Ortalama Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrtalamaTeklif { get; set; }

        [Display(Name = "Minimum Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumTeklif { get; set; }

        [Display(Name = "Maximum Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumTeklif { get; set; }

        [Display(Name = "Tahmini Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TahminiTeklif { get; set; }

        [Display(Name = "Standart Sapma")]
        public double StandartSapma { get; set; }

        [Display(Name = "Medyan Teklif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MedyanTeklif { get; set; }
    }
}