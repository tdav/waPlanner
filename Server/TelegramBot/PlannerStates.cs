namespace waPlanner.TelegramBot
{
    public enum UserRoles
    {
        SUPER_ADMIN = 1,
        ADMIN = 2,
        STAFF = 3,
    }
    public enum PlannerStates
    {
        NONE = 0,
        MAIN_MENU = 1,
        SPECIALIZATION = 2,
        ORGANIZATION = 3,
        CATEGORY = 4,
        STUFF = 5,
        CHOOSE_DATE = 6,
        CHOOSE_TIME = 7,
        PHONE = 8,
        USERNAME = 9,
        ADD_FAVORITES = 10,

        FAVORITES = 21,
        SELECT_FAVORITES = 22,

        SETTINGS = 31,
        PREPARE_CHANGE_NAME = 32,
        CHANGE_NAME = 33,
        CHANGE_PHONE = 34,
        CHANGE_LANG = 35
    }
}
