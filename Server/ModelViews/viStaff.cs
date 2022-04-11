using System;

namespace waPlanner.ModelViews
{
    public class viStaff
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNum { get; set; }
        public DateTime? BirthDay { get; set; }

        public int? CategoryId { get; set; }
        public string Category { get; set; }

        public int? OrganizationId { get; set; }
        public string Organization { get; set; }

        public int UserTypeId { get; set; }
        public string UserType { get; set; }

        public long? TelegramId { get; set; }
        public bool? Online { get; set; }
        public DateTime? Experience { get; set; }
        public int[] Availability { get; set; }
        public DateTime CreateDate { get; set; }
        public string Photo { get; set; }
        public string Password { get; set; }
        public int? Status { get; set; }
        public string Gender { get; set; }
    }
}
