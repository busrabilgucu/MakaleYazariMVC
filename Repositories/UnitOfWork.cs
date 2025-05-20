using Sinav_Busra.Data;
using Sinav_Busra.Models;
using System;
using System.Threading.Tasks;

namespace Sinav_Busra.Repositories
{
    /// <summary>
    /// Unit of Work tasarım deseni implementasyonu
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<Article> _articleRepository;
        private IGenericRepository<Category> _categoryRepository;
        private IGenericRepository<ArticleCategory> _articleCategoryRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Repository'lerin encapsulation ile kullanımı
        public IGenericRepository<Article> Articles => 
            _articleRepository ??= new GenericRepository<Article>(_context);

        public IGenericRepository<Category> Categories => 
            _categoryRepository ??= new GenericRepository<Category>(_context);

        public IGenericRepository<ArticleCategory> ArticleCategories => 
            _articleCategoryRepository ??= new GenericRepository<ArticleCategory>(_context);

        /// <summary>
        /// Context'i dispose eder
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Değişiklikleri veritabanına kaydeder
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Hata yönetimi ve loglama
                throw new Exception($"Değişiklikler kaydedilirken hata oluştu: {ex.Message}", ex);
            }
        }
    }
} 