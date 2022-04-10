using System;
using waPlanner.TelegramBot;

namespace waPlanner.ModelViews
{
    public class TelegramBotValuesModel
    {
        public string Specialization { get; set; }
        public string Organization { get; set; }
        public string Category { get; set; }
        public string Stuff { get; set; }
        public DateTime Calendar { get; set; }
        public string Time { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }

        public PlannerStates State { get; set; }

    }
}
