using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spCategory : NameBaseModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int? GlobalCategoryId { get; set; }
        public virtual spGlobalCategory GlobalCategory { get; set; }
    }
}
