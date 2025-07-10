using System.ComponentModel.DataAnnotations;

namespace Mesfel.Models
{
    public class IhaleKarsilastirmaSonucu
    {
        public Ihale Ihale1 { get; set; }
        public Ihale Ihale2 { get; set; }
        public IhaleAnaliz Analiz1 { get; set; }
        public IhaleAnaliz Analiz2 { get; set; }

        [Display(Name = "Benzerlik Oranı")]
        [Range(0, 1)]
        public decimal BenzerlikOrani { get; set; }

        [Display(Name = "Farklılıklar")]
        public List<Farklilik> Farkliliklar { get; set; } = new List<Farklilik>();

        [Display(Name = "Ortak Özellikler")]
        public List<OrtakOzellik> OrtakOzellikler { get; set; } = new List<OrtakOzellik>();

        [Display(Name = "Tavsiyeler")]
        public List<string> Tavsiyeler { get; set; } = new List<string>();
    }




}