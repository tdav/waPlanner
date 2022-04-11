using System;

namespace waPlanner.ModelViews
{
    public class viMiniScheduler
    {
        public int SchedulerId { get; set; }
        public int? UserId { get; set; }
        public string User { get; set; }

        public int? StaffId { get; set; }
        public string Staff { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string AdInfo { get; set; }
    }
}
