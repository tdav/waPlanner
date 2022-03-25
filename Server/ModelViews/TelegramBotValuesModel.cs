using waPlanner.Database.Models;
using waPlanner.TelegramBot;

namespace waPlanner.ModelViews
{
    public class TelegramBotValuesModel
    {
        public string Category { get; set; }

        public PlannerStates State { get; set; }

    }
}
