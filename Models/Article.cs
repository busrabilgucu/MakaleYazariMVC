using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sinav_Busra.Models
{
    /// <summary>
    /// Makale entity sınıfı
    /// </summary>
    public class Article : BaseEntity
    {
        public Article()
        {
            ArticleCategories = new List<ArticleCategory>();
        }
        
        // Makale başlığı
        [Required(ErrorMessage = "Başlık alanı zorunludur")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Title { get; set; }
        
        // Makale içeriği
        [Required(ErrorMessage = "İçerik alanı zorunludur")]
        public string Content { get; set; }
        
        // Makaleyi yazan kullanıcının ID'si
        public string ApplicationUserId { get; set; }
        
        // Makaleyi yazan kullanıcı - Navigation property
        public ApplicationUser Author { get; set; }
        
        // Makalenin kategorileri - Navigation property
        public ICollection<ArticleCategory> ArticleCategories { get; set; }
    }
} 