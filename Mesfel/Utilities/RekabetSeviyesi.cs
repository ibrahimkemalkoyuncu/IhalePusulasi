using System.ComponentModel.DataAnnotations;

namespace Mesfel.Utilities
{
    public enum RekabetSeviyesi
    {
        [Display(Name = "Düşük")]
        Dusuk = 1,
        [Display(Name = "Orta")]
        Orta = 2,
        [Display(Name = "Yüksek")]
        Yuksek = 3,
        [Display(Name = "Çok Yüksek")]
        CokYuksek = 4
    }
}
