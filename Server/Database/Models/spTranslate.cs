using System.ComponentModel.DataAnnotations.Schema;

namespace waPlanner.Database.Models
{
    public class spTranslate
    {
        public int Id { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string Words { get; set; }
    }

    public class Languages
    {
        public string Uz { get; set; }
        public string Ru { get; set; }

    }
}
