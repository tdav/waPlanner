namespace waPlanner.ModelViews
{
    public class viAppointmentsModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Symptoms { get; set; }
    }
}
