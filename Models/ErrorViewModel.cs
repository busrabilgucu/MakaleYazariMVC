namespace Sinav_Busra.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        // Hata mesajı
        public string ErrorMessage { get; set; }
    }
}
