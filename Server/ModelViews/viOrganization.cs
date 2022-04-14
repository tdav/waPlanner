namespace waPlanner.ModelViews
{
    public class viOrganization
    {
        public int Id { get; set; }
        public long? ChatId { get; set; }
        public string Name { get; set; }
        public string address { get; set; }

        public int? SpecializationId { get; set; }
        public string Specialization { get; set; }

        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public int? Status { get; set; }
    }
}
