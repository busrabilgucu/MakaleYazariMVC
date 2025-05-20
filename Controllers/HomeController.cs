using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sinav_Busra.Models;
using Sinav_Busra.Models.ViewModels;
using Sinav_Busra.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sinav_Busra.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ana sayfa - Tüm makaleleri listeler
        /// </summary>
        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            try
            {
                // Sayfa boyutu ve başlangıç indeksi
                int pageSize = 10;
                int skip = (page - 1) * pageSize;
                
                // Makaleleri sorgula
                var query = _unitOfWork.Articles.Query()
                    .Include(a => a.Author)
                    .Include(a => a.ArticleCategories)
                    .ThenInclude(ac => ac.Category)
                    .Where(a => a.IsActive);

                // Kategori filtresi varsa uygula
                if (categoryId.HasValue)
                {
                    query = query.Where(a => a.ArticleCategories.Any(ac => ac.CategoryId == categoryId.Value));
                }

                // Sıralama işlemini filtreleme sonrası uygula
                var orderedQuery = query.OrderByDescending(a => a.CreatedDate);

                // Toplam sonuç ve sayfa sayısını hesapla
                int totalItems = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                
                // Sayfadaki makaleleri al
                var articles = await orderedQuery.Skip(skip).Take(pageSize).ToListAsync();
                
                // Tüm kategorileri al
                var categories = await _unitOfWork.Categories.GetAllAsync();
                
                // View model oluştur
                var model = new ArticleListViewModel
                {
                    Articles = articles.Select(a => new ArticleViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Content = a.Content,
                        AuthorName = a.Author?.FullName ?? a.Author?.UserName,
                        CreatedDate = a.CreatedDate,
                        CategoryNames = a.ArticleCategories.Select(ac => ac.Category.Name).ToList()
                    }).ToList(),
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    SelectedCategoryId = categoryId,
                    Categories = categories.OrderBy(c => c.Name).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                // Hata durumunda
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = "Makaleler yüklenirken bir hata oluştu: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Makale detayını gösterir
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Makaleyi ilişkili verilerle birlikte getir
                var article = await _unitOfWork.Articles.Query()
                    .Include(a => a.Author)
                    .Include(a => a.ArticleCategories)
                    .ThenInclude(ac => ac.Category)
                    .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
                
                if (article == null)
                {
                    return NotFound();
                }
                
                // View model oluştur
                var model = new ArticleViewModel
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    AuthorName = article.Author?.FullName ?? article.Author?.UserName,
                    CreatedDate = article.CreatedDate,
                    CategoryNames = article.ArticleCategories.Select(ac => ac.Category.Name).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                // Hata durumunda
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = "Makale detayı yüklenirken bir hata oluştu: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Hata sayfası
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
