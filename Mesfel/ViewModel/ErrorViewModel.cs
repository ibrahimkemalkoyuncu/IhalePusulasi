namespace Mesfel.ViewModel
{
    /// <summary>
    /// Hata sayfas� i�in view model
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
