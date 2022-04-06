using System;
using System.ComponentModel.DataAnnotations;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class tbScheduler : BaseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual tbUser User { get; set; }

        public int DoctorId { get; set; }
        public virtual tbUser Doctor { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        [StringLength(200)]
        public string AdInfo { get; set; }

        public int CategoryId { get; set; }
        public virtual spCategory Category { get; set; }

        public int? OrganizationId { get; set; }
        public virtual tbOrganization Organization { get; set; }
    }
}
