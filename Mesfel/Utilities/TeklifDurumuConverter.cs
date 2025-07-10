using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mesfel.Utilities
{
    public class TeklifDurumuConverter : ValueConverter<TeklifDurumu, string>
    {
        public TeklifDurumuConverter() : base(
            v => ConvertToDb(v),
            v => ConvertFromDb(v),
            new ConverterMappingHints(size: 20))
        {
        }

        private static string ConvertToDb(TeklifDurumu value)
        {
            return value switch
            {
                TeklifDurumu.Verildi => "VERILDI",
                TeklifDurumu.Degerlendiriliyor => "DEGERLENDIRILIYOR",
                TeklifDurumu.KabulEdildi => "KABUL",
                TeklifDurumu.Reddedildi => "REDDEDILDI",
                TeklifDurumu.Gecersiz => "GECERSIZ",
                TeklifDurumu.IptalEdildi => "IPTAL",
                _ => "TANIMSIZ"
            };
        }

        private static TeklifDurumu ConvertFromDb(string value)
        {
            return value switch
            {
                "VERILDI" => TeklifDurumu.Verildi,
                "DEGERLENDIRILIYOR" => TeklifDurumu.Degerlendiriliyor,
                "KABUL" => TeklifDurumu.KabulEdildi,
                "REDDEDILDI" => TeklifDurumu.Reddedildi,
                "GECERSIZ" => TeklifDurumu.Gecersiz,
                "IPTAL" => TeklifDurumu.IptalEdildi,
                _ => throw new InvalidOperationException($"Bilinmeyen teklif durumu: {value}")
            };
        }
    }
}
