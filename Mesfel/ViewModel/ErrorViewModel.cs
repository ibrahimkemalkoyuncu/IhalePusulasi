namespace Mesfel.ViewModel
{
    /// <summary>
    /// Hata sayfasý için view model
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
