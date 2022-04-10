using System;

namespace waPlanner.ModelViews
{
    public class viScheduler
    {
        public int? UserId { get; set; }
        public string User { get; set; }

        public int? StaffId { get; set; }
        public string Staff { get; set; }

        public int? CategoryId { get; set; }
        public string Category { get; set; }

        public int? OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public string AdInfo { get; set; }
        public int? Status { get;  set; }
    }
}
