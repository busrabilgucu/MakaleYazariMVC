using System.ComponentModel.DataAnnotations;

namespace Sinav_Busra.Models.ViewModels
{
    /// <summary>
    /// Kullanıcı giriş view model
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        
        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
} 