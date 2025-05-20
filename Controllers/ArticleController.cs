using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// <summary>
    /// Makale işlemleri controller'ı
    /// </summary>
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArticleController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        /// <summary>
        /// Kullanıcının makalelerini listeler
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToAction("Index", "Home");
                }

                // Debug: Kullanıcı ID'sini yazdır
                Debug.WriteLine($"Kullanıcı ID: {user.Id}");

                // Kullanıcıya ait makaleleri sorgula
                var query = _unitOfWork.Articles.Query()
                    .Include(a => a.ArticleCategories)
                    .ThenInclude(ac => ac.Category)
                    .Where(a => a.ApplicationUserId == user.Id && a.IsActive);
                    
                // Sıralama işlemini filtreleme sonrası uygula
                var articles = await query.OrderByDescending(a => a.CreatedDate).ToListAsync();

                // Debug: Bulunan makale sayısını yazdır
                Debug.WriteLine($"Bulunan makale sayısı: {articles.Count}");

                // View model oluştur
                var model = articles.Select(a => new ArticleViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedDate = a.CreatedDate,
                    CategoryNames = a.ArticleCategories.Select(ac => ac.Category.Name).ToList()
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Index hata: {ex.Message}");
                TempData["ErrorMessage"] = $"Makaleler listelenirken hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Yeni makale oluşturma sayfası
        /// </summary>
        public async Task<IActionResult> Create()
        {
            try
            {
                // Kategorileri getir
                var categories = await _unitOfWork.Categories.GetAllAsync();
                if (categories == null || !categories.Any())
                {
                    TempData["ErrorMessage"] = "Kategoriler yüklenemedi.";
                    return RedirectToAction(nameof(Index));
                }

                Debug.WriteLine($"Yüklenen kategori sayısı: {categories.Count()}");
                
                // View model oluştur
                var model = new ArticleViewModel
                {
                    AvailableCategories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name ?? "Kategori"
                    }).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Create GET hata: {ex.Message}");
                TempData["ErrorMessage"] = $"Kategori bilgileri yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Yeni makale oluşturma - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel model)
        {
            try
            {
                Debug.WriteLine($"Form gönderildi: Başlık={model.Title}, Kategori Sayısı={model.SelectedCategoryIds?.Count ?? 0}");
                
                // Form gönderiminde kategori seçilip seçilmediğini kontrol et
                if (model.SelectedCategoryIds == null || !model.SelectedCategoryIds.Any())
                {
                    ModelState.AddModelError("SelectedCategoryIds", "En az bir kategori seçmelisiniz.");
                    Debug.WriteLine("Hata: Kategori seçilmemiş");
                }

                if (ModelState.IsValid)
                {
                    // Giriş yapan kullanıcıyı al
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        Debug.WriteLine("Hata: Kullanıcı bilgisi alınamadı");
                        return Unauthorized();
                    }
                    
                    Debug.WriteLine($"Kullanıcı: {user.UserName}, ID: {user.Id}");
                    
                    // Yeni makale oluştur
                    var article = new Article
                    {
                        Title = model.Title,
                        Content = model.Content,
                        ApplicationUserId = user.Id,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        ArticleCategories = new List<ArticleCategory>()
                    };
                    
                    // Makaleyi veritabanına ekle
                    _unitOfWork.Articles.Add(article);
                    
                    // İlk önce makaleyi kaydet - ID alabilmek için
                    var saveResult = await _unitOfWork.SaveChangesAsync();
                    Debug.WriteLine($"Makale kaydedildi, değişiklik sayısı: {saveResult}, Makale ID: {article.Id}");
                    
                    // Seçilen kategorileri ekle
                    if (model.SelectedCategoryIds != null && model.SelectedCategoryIds.Count > 0)
                    {
                        Debug.WriteLine($"Seçilen kategori sayısı: {model.SelectedCategoryIds.Count}");
                        foreach (var categoryId in model.SelectedCategoryIds)
                        {
                            var articleCategory = new ArticleCategory
                            {
                                ArticleId = article.Id,
                                CategoryId = categoryId
                            };
                            
                            _unitOfWork.ArticleCategories.Add(articleCategory);
                            Debug.WriteLine($"Kategori eklendi: {categoryId}");
                        }
                        
                        // Kategorileri kaydet
                        saveResult = await _unitOfWork.SaveChangesAsync();
                        Debug.WriteLine($"Kategoriler kaydedildi, değişiklik sayısı: {saveResult}");
                    }
                    
                    TempData["SuccessMessage"] = "Makale başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }
                
                Debug.WriteLine("Model geçerli değil");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"Model hatası: {error.ErrorMessage}");
                }
                
                // Model hatalıysa, kategorileri tekrar yükle ve view'e döndür
                var categories = await _unitOfWork.Categories.GetAllAsync();
                model.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name ?? "Kategori",
                    Selected = model.SelectedCategoryIds != null && model.SelectedCategoryIds.Contains(c.Id)
                }).ToList();
                
                return View(model);
            }
            catch (Exception ex)
            {
                // Hata ayrıntılarını logla
                Debug.WriteLine($"Create POST hata: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                TempData["ErrorMessage"] = $"Makale oluşturulurken hata oluştu: {ex.Message}";
                
                // Kategorileri tekrar yükle
                try
                {
                    var categories = await _unitOfWork.Categories.GetAllAsync();
                    model.AvailableCategories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name ?? "Kategori",
                        Selected = model.SelectedCategoryIds != null && model.SelectedCategoryIds.Contains(c.Id)
                    }).ToList();
                }
                catch (Exception loadEx)
                {
                    Debug.WriteLine($"Kategori yükleme hatası: {loadEx.Message}");
                    // Kategori yükleme hatası - boş liste kullan
                    model.AvailableCategories = new List<SelectListItem>();
                }
                
                return View(model);
            }
        }

        /// <summary>
        /// Makale düzenleme sayfası
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                
                // Makaleyi getir
                var article = await _unitOfWork.Articles.Query()
                    .Include(a => a.ArticleCategories)
                    .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUserId == user.Id);
                
                if (article == null)
                {
                    return NotFound();
                }
                
                // Kategorileri getir
                var categories = await _unitOfWork.Categories.GetAllAsync();
                
                // Makalenin seçili kategori ID'lerini al
                var selectedCategoryIds = article.ArticleCategories
                    .Select(ac => ac.CategoryId)
                    .ToList();
                
                // View model oluştur
                var model = new ArticleViewModel
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    SelectedCategoryIds = selectedCategoryIds,
                    AvailableCategories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = selectedCategoryIds.Contains(c.Id)
                    }).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale düzenleme sayfası yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Makale düzenleme - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                
                if (ModelState.IsValid)
                {
                    // Giriş yapan kullanıcıyı al
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return Unauthorized();
                    }
                    
                    // Makaleyi getir
                    var article = await _unitOfWork.Articles.Query()
                        .Include(a => a.ArticleCategories)
                        .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUserId == user.Id);
                    
                    if (article == null)
                    {
                        return NotFound();
                    }
                    
                    // Makaleyi güncelle
                    article.Title = model.Title;
                    article.Content = model.Content;
                    article.UpdatedDate = DateTime.Now;
                    
                    _unitOfWork.Articles.Update(article);
                    
                    // Mevcut kategorileri sil
                    var existingCategories = await _unitOfWork.ArticleCategories
                        .GetByConditionAsync(ac => ac.ArticleId == id);
                    
                    _unitOfWork.ArticleCategories.DeleteRange(existingCategories);
                    
                    // Yeni kategorileri ekle
                    if (model.SelectedCategoryIds != null && model.SelectedCategoryIds.Count > 0)
                    {
                        foreach (var categoryId in model.SelectedCategoryIds)
                        {
                            var articleCategory = new ArticleCategory
                            {
                                ArticleId = id,
                                CategoryId = categoryId
                            };
                            
                            _unitOfWork.ArticleCategories.Add(articleCategory);
                        }
                    }
                    
                    await _unitOfWork.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Makale başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                
                // Model hatalıysa, kategorileri tekrar yükle ve view'e döndür
                var categories = await _unitOfWork.Categories.GetAllAsync();
                model.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale güncellenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Makale silme onay sayfası
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                
                // Makaleyi getir
                var article = await _unitOfWork.Articles.Query()
                    .Include(a => a.ArticleCategories)
                    .ThenInclude(ac => ac.Category)
                    .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUserId == user.Id);
                
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
                    CreatedDate = article.CreatedDate,
                    CategoryNames = article.ArticleCategories.Select(ac => ac.Category.Name).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale silme sayfası yüklenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Makale silme onayı - POST
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                
                // Makaleyi getir
                var article = await _unitOfWork.Articles.Query()
                    .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUserId == user.Id);
                
                if (article == null)
                {
                    return NotFound();
                }
                
                // Makaleyi soft delete (IsActive = false) olarak işaretle
                article.IsActive = false;
                article.UpdatedDate = DateTime.Now;
                
                _unitOfWork.Articles.Update(article);
                await _unitOfWork.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Makale başarıyla silindi!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale silinirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 