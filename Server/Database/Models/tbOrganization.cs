using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class tbOrganization: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public long ChatId { get; set; }
        [Required]
        [StringLength(150)]
        public string Address { get; set; }
        [Required]
        public float  Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public int TypeId { get; set; }
        public virtual spOrganizationType Type { get; set; }
    }
}
