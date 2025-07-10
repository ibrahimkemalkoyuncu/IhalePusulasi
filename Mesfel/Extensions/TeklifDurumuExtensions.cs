using Mesfel.Utilities;

namespace Mesfel.Extensions
{
    // UI Katmanı için
    public static class TeklifDurumuExtensions
    {
        public static string ToDisplayString(this TeklifDurumu status)
        {
            return status switch
            {
                TeklifDurumu.Verildi => "Verildi",
                TeklifDurumu.Degerlendiriliyor => "Değerlendiriliyor",
                TeklifDurumu.KabulEdildi => "Kabul Edildi",
                TeklifDurumu.Reddedildi => "Reddedildi",
                TeklifDurumu.Gecersiz => "Geçersiz",
                _ => status.ToString()
            };
        }

        public static string ToCssClass(this TeklifDurumu status)
        {
            return status switch
            {
                TeklifDurumu.Verildi => "badge bg-primary",
                TeklifDurumu.Degerlendiriliyor => "badge bg-warning",
                TeklifDurumu.KabulEdildi => "badge bg-success",
                TeklifDurumu.Reddedildi => "badge bg-danger",
                TeklifDurumu.Gecersiz => "badge bg-secondary",
                _ => "badge"
            };
        }
    }
}
