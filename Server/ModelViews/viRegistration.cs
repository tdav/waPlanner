using System;

namespace waPlanner.ModelViews
{
    public class viRegistration
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public DateTime BreakTimeStart { get; set; }
        public DateTime BreakTimeEnd { get; set; }
        public DateTime WorkStart { get; set; }
        public DateTime WorkEnd { get; set; }
        public int SpecializationId { get; set; }

    }
}
