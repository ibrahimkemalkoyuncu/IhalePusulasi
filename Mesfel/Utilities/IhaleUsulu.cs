using System.ComponentModel.DataAnnotations;

namespace Mesfel.Utilities
{
    // İhale Usulü Enum
    public enum IhaleUsulu
    {
        [Display(Name = "Açık İhale")]
        AcikIhale = 1,

        [Display(Name = "Belli İstekliler Arası İhale")]
        BelliIsteklilerArasi = 2,

        [Display(Name = "Pazarlık Usulü")]
        PazarlikUsulu = 3,

        [Display(Name = "Belli İşyerlerine Davetli")]
        BelliIsyerlerineDavetli = 3
    }
}
