using System;

namespace waPlanner.ModelViews
{
    public class viPatient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AdInfo { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Password { get; set; }
        public byte Status { get; set; }
    }
    public class viPatientStatus
    {
        public byte Status { get; set; }
    }
}
