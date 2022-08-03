using System;

namespace waPlanner.ModelViews
{
    public class viOrganization
    {
        public int Id { get; set; }
        public long? ChatId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? BreakTimeStart { get; set; }
        public DateTime? BreakTimeEnd { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }

        public int? SpecializationId { get; set; }
        public string Specialization { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? Status { get; set; }
        public string MessageRu { get; set; }
        public string MessageUz { get; set; }
        public string MessageLt { get; set; }
        public string PhotoPath { get; set; }
        public string OrganizationInfo { get; set; }
        public int? Rating { get; set; }
    }
}
