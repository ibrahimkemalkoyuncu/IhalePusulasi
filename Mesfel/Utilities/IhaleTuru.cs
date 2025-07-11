using System.ComponentModel.DataAnnotations;

namespace Mesfel.Utilities
{
    // İhale Türü Enum
    public enum IhaleTuru
    {
        [Display(Name = "Mal Alımı")]
        MalAlimi = 1,

        [Display(Name = "Hizmet Alımı")]
        HizmetAlimi = 2,

        [Display(Name = "Yapım İşi")]
        YapimIsi = 3,

        [Display(Name = "Danışmanlık Hizmeti")]
        Danismanlik = 4,

        [Display(Name = "Özel Hizmeti")]
        Ozel = 5,

        [Display(Name = "Kamu Hizmeti")]
        Kamu = 5


    }
}
