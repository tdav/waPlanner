using System;

namespace waPlanner.ModelViews
{
    public class viOrganization
    {
        public int Id { get; set; }
        public long? ChatId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? DinnerTimeStart { get; set; }
        public DateTime? DinnerTimeEnd { get; set; }

        public DateTime WorkTimeStart { get; set; }
        public DateTime WorkTimeEnd { get; set; }

        public int? SpecializationId { get; set; }
        public string Specialization { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? Status { get; set; }
        public string MessageRu { get; set; }
        public string MessageUz { get; set; }
        public string MessageLt { get; set; }
    }
}
