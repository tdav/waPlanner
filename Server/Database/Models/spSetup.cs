using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using waPlanner.ModelViews;


namespace waPlanner.Database.Models
{
    public class spSetup : BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? StaffId { get; set; }
        public virtual tbStaff Staff { get; set; }

        public int? UserId { get; set; }
        public virtual tbUser User { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public string Text { get; set; }
    }
}
