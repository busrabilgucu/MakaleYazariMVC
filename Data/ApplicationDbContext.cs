using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sinav_Busra.Models;
using System;

namespace Sinav_Busra.Data
{
    /// <summary>
    /// Uygulama veritabanı bağlantı sınıfı
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet tanımlamaları
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ArticleCategory ilişki konfigürasyonu
            modelBuilder.Entity<ArticleCategory>()
                .HasKey(ac => new { ac.ArticleId, ac.CategoryId }); // Composite key tanımı

            modelBuilder.Entity<ArticleCategory>()
                .HasOne(ac => ac.Article)
                .WithMany(a => a.ArticleCategories)
                .HasForeignKey(ac => ac.ArticleId);

            modelBuilder.Entity<ArticleCategory>()
                .HasOne(ac => ac.Category)
                .WithMany(c => c.ArticleCategories)
                .HasForeignKey(ac => ac.CategoryId);

            // Makale-Kullanıcı ilişkisi
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kategori seed data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Teknoloji", Description = "Teknoloji ile ilgili makaleler", CreatedDate = DateTime.Now },
                new Category { Id = 2, Name = "Eğitim", Description = "Eğitim ile ilgili makaleler", CreatedDate = DateTime.Now },
                new Category { Id = 3, Name = "Sağlık", Description = "Sağlık ile ilgili makaleler", CreatedDate = DateTime.Now },
                new Category { Id = 4, Name = "Spor", Description = "Spor ile ilgili makaleler", CreatedDate = DateTime.Now },
                new Category { Id = 5, Name = "Bilim", Description = "Bilim ile ilgili makaleler", CreatedDate = DateTime.Now }
            );
        }
    }
} 