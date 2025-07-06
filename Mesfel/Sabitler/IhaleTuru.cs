using System.ComponentModel.DataAnnotations;

namespace Mesfel.Sabitler
{
    // İhale Türü Enum
    public enum IhaleTuru
    {
        [Display(Name = "Mal Alımı")]
        MalAlimi = 1,
        [Display(Name = "Hizmet Alımı")]
        HizmetAlimi = 2,
        [Display(Name = "Yapım İşi")]
        YapimIsi = 3
    }
}
