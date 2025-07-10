using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Mesfel.Services
{
    public interface IEnumService
    {
        IEnumerable<KeyValuePair<int, string>> GetEnumValues<TEnum>() where TEnum : Enum;
        string GetDisplayName(Enum value);
        string GetDescription(Enum value);
    }

    public class EnumService : IEnumService
    {
        public IEnumerable<KeyValuePair<int, string>> GetEnumValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new KeyValuePair<int, string>(
                    Convert.ToInt32(e),
                    GetDisplayName(e)));
        }

        public string GetDisplayName(Enum value)
        {
            return value.GetType()
                       .GetMember(value.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       ?.Name ?? value.ToString();
        }

        public string GetDescription(Enum value)
        {
            return value.GetType()
                       .GetMember(value.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       ?.Description ?? string.Empty;
        }
    }
}
