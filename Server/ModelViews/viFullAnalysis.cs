using waPlanner.Database.Models;

namespace waPlanner.ModelViews
{
    public class viFullAnalysis
    {
        public int Id { get; set; }
        public virtual tbUser User { get; set; }
        public int StaffId { get; set; }
        public int OrganizationId { get; set; }
        public string FileUrl { get; set; }
        public string AdInfo { get; set; }
    }
}
