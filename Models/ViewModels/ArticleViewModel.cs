using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sinav_Busra.Models.ViewModels
{
    /// <summary>
    /// Makale form view model
    /// </summary>
    public class ArticleViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Başlık alanı zorunludur")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        [Display(Name = "Başlık")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "İçerik alanı zorunludur")]
        [Display(Name = "İçerik")]
        public string Content { get; set; }
        
        public string ApplicationUserId { get; set; }
        
        [Display(Name = "Yazar")]
        public string AuthorName { get; set; }
        
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Kategoriler")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        
        // Kategori seçimi için kullanılacak
        public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
        
        // Görüntüleme için kategorilerin isim listesi
        public List<string> CategoryNames { get; set; } = new List<string>();
    }
} 