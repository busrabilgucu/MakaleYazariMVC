using System;

namespace Sinav_Busra.Models
{
    /// <summary>
    /// Tüm entity sınıfları için temel bir abstract sınıf
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        
        // Oluşturulma tarihi
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Güncelleme tarihi
        public DateTime? UpdatedDate { get; set; }
        
        // Aktif/Pasif durumu
        public bool IsActive { get; set; } = true;
    }
} 