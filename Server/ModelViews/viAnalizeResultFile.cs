using Microsoft.AspNetCore.Http;

namespace waPlanner.ModelViews
{
    public class viAnalizeResultFile
    {
        public IFormFile FileData { get; set; }
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public string AdInfo { get; set; }
    }
}
