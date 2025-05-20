using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sinav_Busra.Repositories
{
    /// <summary>
    /// Generic Repository interface
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        // Tüm entityleri getir
        Task<IEnumerable<T>> GetAllAsync();
        
        // Belirli koşulları sağlayan tüm entityleri getir
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> expression);
        
        // ID'ye göre entity getir
        Task<T> GetByIdAsync(int id);
        
        // Tek bir entity getir
        Task<T> GetSingleAsync(Expression<Func<T, bool>> expression);
        
        // Entity ekle
        void Add(T entity);
        
        // Entity güncelle
        void Update(T entity);
        
        // Entity sil
        void Delete(T entity);
        
        // Toplu entity ekleme
        void AddRange(IEnumerable<T> entities);
        
        // Toplu entity silme
        void DeleteRange(IEnumerable<T> entities);
        
        // Belirli bir sorgu üzerinde işlem yapma imkanı için.
        IQueryable<T> Query();
    }
} 