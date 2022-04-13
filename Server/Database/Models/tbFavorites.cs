using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using waPlanner.ModelViews;


namespace waPlanner.Database.Models
{
    public class tbFavorites : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual tbUser User { get; set; }
        public long TelegramId { get; set; }

        public int StaffId { get; set; }
        public virtual tbStaff Staff { get; set; }

        public int OrganizationId { get; set; }
        public virtual spOrganization Organization { get; set; }
    }
}
