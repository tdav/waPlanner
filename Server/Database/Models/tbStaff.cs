using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    /// <summary>
    /// Тизим фойдаланувчилар (Доктор, Официант)
    /// </summary>
    public class tbStaff : BaseModel
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

        [IndexColumn]
        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public DateTime? BirthDay { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        [Required]
        public int RoleId { get; set; }
        public virtual spRole Role { get; set; }

        public int? CategoryId { get; set; }

        public virtual spCategory Category { get; set; }

        public int? OrganizationId { get; set; }

        public virtual spOrganization Organization { get; set; }

        public long? TelegramId { get; set; }

        public bool? Online { get; set; }

        public DateTime? Experience { get; set; }

        public int[] Availability { get; set; }


        [StringLength(256)]
        public string PhotoUrl { get; set; }

        [StringLength(150)]
        public string AdInfo { get; set; } 
        public int PeriodTime { get; set; }
        public int? Rating { get; set; }
    }
}
