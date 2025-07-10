using Mesfel.Services;
using Mesfel.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mesfel.Helpers
{
    public static class EnumHelper
    {
        public static SelectList ToSelectList<TEnum>(this IEnumService enumService)
            where TEnum : Enum
        {
            var values = enumService.GetEnumValues<TEnum>();
            return new SelectList(values, "Key", "Value");
        }
    }

    // Kullanımı:
    //var teklifDurumlari = _enumService.ToSelectList<TeklifDurumu>();
}
