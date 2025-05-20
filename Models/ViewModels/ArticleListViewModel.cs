using System.Collections.Generic;

namespace Sinav_Busra.Models.ViewModels
{
    /// <summary>
    /// Makale liste view model
    /// </summary>
    public class ArticleListViewModel
    {
        // Makaleler listesi
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
        
        // Filtreleme için seçili kategori ID'si
        public int? SelectedCategoryId { get; set; }
        
        // Kategoriler listesi
        public List<Category> Categories { get; set; } = new List<Category>();
        
        // Sayfalama bilgisi
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }
} 