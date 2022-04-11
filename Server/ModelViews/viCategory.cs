namespace waPlanner.ModelViews
{
    public class viCategory
    {
        public int Id { get; set; }
        public string NameUz { get; set; }
        public string NameRu { get; set; }
        public string NameLt { get; set; }

        public int? OrganizationId { get; set; }
        public string Organization { get; set; }

        public int? Status { get; set; }

    }
}
