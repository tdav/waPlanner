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
        ORGANIZATION = 1,
        CATEGORY = 2,
        STUFF = 3,
        CHOOSE_DATE = 4,
        CHOOSE_TIME = 5,
        PHONE = 6,
        USERNAME = 7
    }
}
