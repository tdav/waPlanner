using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    /// <summary>
    /// Пользователь системы
    /// Продавец 
    /// Доставщик
    /// Потребитель
    /// </summary>
    public class tbUser : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Patronymic { get; set; }

        [IndexColumn]
        [StringLength(20)]
        public string PhoneNum { get; set; }

        [IndexColumn]
        [Required]
        [StringLength(50)]
        public string? Password { get; set; }
        public DateTime? BirthDay { get; set; }

        [Required]
        public int UserTypeId { get; set; }
        public virtual spUserType UserType { get; set; }

        public int? CategoryId { get; set; }
        public virtual spCategory Category { get; set; }

        public long? TelegramId { get; set; }

        public bool? Online { get; set; }
        public DateTime? Experience { get; set; }
        public int[] Availability { get; set; }

    }
}
