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
        SPECIALIZATION = 1,
        ORGANIZATION = 2,
        CATEGORY = 3,
        STUFF = 4,
        CHOOSE_DATE = 5,
        CHOOSE_TIME = 6,
        PHONE = 7,
        USERNAME = 8,
        ADD_FAVORITES = 9,

        FAVORITES = 21,
    }
}
