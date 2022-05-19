using System.ComponentModel.DataAnnotations;
using waPlanner.ModelViews;


namespace waPlanner.Database.Models
{
    public class tbRating: BaseModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual tbUser User { get; set; }

        public int? StaffId { get; set; }
        public virtual tbStaff Staff { get; set; }

        public int? OrganizationId { get; set; }
        public virtual spOrganization Organization { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
