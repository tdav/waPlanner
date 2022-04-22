using System;

namespace waPlanner.ModelViews
{
    public class viEvents
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public string Staff { get; set; }

        public int UserId { get; set; }
        public string User { get; set; }
        public string UserPhoneNum { get; set; }
        public string AdInfo { get; set; }
        public DateTime Start { get ; set; }
        public DateTime End { get; set; }
    }
}
