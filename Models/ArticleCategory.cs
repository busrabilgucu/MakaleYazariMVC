namespace Sinav_Busra.Models
{
    /// <summary>
    /// Makale ve Kategori arasındaki many-to-many ilişkisi için ara tablo
    /// </summary>
    public class ArticleCategory
    {
        public ArticleCategory()
        {
            Article = null;
            Category = null;
        }
        
        // Makale ID'si
        public int ArticleId { get; set; }
        
        // Makale navigasyon property'si
        public Article Article { get; set; }
        
        // Kategori ID'si
        public int CategoryId { get; set; }
        
        // Kategori navigasyon property'si
        public Category Category { get; set; }
    }
} 