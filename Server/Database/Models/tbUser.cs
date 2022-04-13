using System;
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

        [StringLength(100)]
        public string Patronymic { get; set; }
         
        [IndexColumn]
        [StringLength(20)]
        public string PhoneNum { get; set; }
        public DateTime? BirthDay { get; set; }
        public long? TelegramId { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }
    }
}
