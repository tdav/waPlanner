using System;

namespace waPlanner.ModelViews
{
    public class SendDocumentsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FilePath { get; set; }
        public long ChatId { get; set; }
        public string Caption { get; set; }
        public string User { get; set; }
    }
}
