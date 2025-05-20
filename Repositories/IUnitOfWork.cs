using Sinav_Busra.Models;
using System;
using System.Threading.Tasks;

namespace Sinav_Busra.Repositories
{
    /// <summary>
    /// Unit of Work tasarım deseni için interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repository'ler
        IGenericRepository<Article> Articles { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<ArticleCategory> ArticleCategories { get; }
        
        // Değişiklikleri veritabanına kaydet
        Task<int> SaveChangesAsync();
    }
} 