using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Sinav_Busra.Models
{
    /// <summary>
    /// Identity kullanıcı sınıfı
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Articles = new List<Article>();
        }
        
        // Kullanıcı adı ve soyadı
        public string FullName { get; set; }
        
        // Kullanıcının makaleleri - Navigation property
        public ICollection<Article> Articles { get; set; }
    }
} 