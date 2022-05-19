using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class tbAnalizeResult : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Url { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual tbUser User { get; set; }

        [StringLength(256)]
        public string AdInfo { get; set; }

        public int? StaffId { get; set; }
        public virtual tbStaff Staff { get; set; }
        
        public int? OrganizationId { get; set; }

        public virtual spOrganization Organization { get; set; }

    }
}
