namespace waPlanner.TelegramBot
{
    public enum UserTypes
    {
        STAFF = 1,
        TELEGRAM_USER = 2,
        SYSTEM_USER = 3
    }
    public enum PlannerStates
    {
        NONE = 0,
        CATEGORY = 1,
        STUFF = 2,
        CHOOSE_DATE = 3,
        CHOOSE_TIME = 4,
        PHONE = 5,
        USERNAME = 6
    }
}
