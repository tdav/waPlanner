using System.Collections.Generic;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spGlobalCategory : NameBaseModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public List<spCategory> Categories { get; set; }
    }
}
