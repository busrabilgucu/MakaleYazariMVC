using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sinav_Busra.Models
{
    /// <summary>
    /// Kategori entity sınıfı
    /// </summary>
    public class Category : BaseEntity
    {
        public Category()
        {
            ArticleCategories = new List<ArticleCategory>();
            Name = string.Empty;
            Description = string.Empty;
        }
        
        // Kategori adı
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }
        
        // Kategori açıklaması
        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Description { get; set; }
        
        // Bu kategoriye ait makaleler - Navigation property
        public ICollection<ArticleCategory> ArticleCategories { get; set; }
    }
} 