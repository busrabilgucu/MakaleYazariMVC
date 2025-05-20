using Microsoft.EntityFrameworkCore;
using Sinav_Busra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sinav_Busra.Repositories
{
    /// <summary>
    /// Generic Repository implementasyonu
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Yeni entity ekler
        /// </summary>
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Birden fazla entity ekler
        /// </summary>
        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <summary>
        /// Entity siler
        /// </summary>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Birden fazla entity siler
        /// </summary>
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Tüm entityleri getirir
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                throw new Exception($"Verileri alırken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Belirli koşulları sağlayan entityleri getirir
        /// </summary>
        public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _dbSet.Where(expression).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Koşullu veri alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ID'ye göre entity getirir
        /// </summary>
        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"ID ile veri alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Belirli koşulu sağlayan tek bir entity getirir
        /// </summary>
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Tekil veri alınırken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// IQueryable döndürerek LINQ sorgularını destekler
        /// </summary>
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Entity günceller
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
} 