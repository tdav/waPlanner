using System.ComponentModel.DataAnnotations;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spRole : BaseModel
    {
        public int Id { get; set; }
                
        [StringLength(20)]
        public string Name { get; set; }
    }
}
