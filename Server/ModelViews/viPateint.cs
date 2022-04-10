using System;

namespace waPlanner.ModelViews
{
    public class viPatient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Gender { get; set; }
        public string AdInfo { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Password { get; set; }
        public int? Status { get; set; }
    }
}
