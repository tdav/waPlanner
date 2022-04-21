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
        FAVORITES = 1,
        SELECT_FAVORITES = 2,

        SETTINGS = 3,
        PREPARE_CHANGE_NAME = 4,
        CHANGE_NAME = 5,
        CHANGE_PHONE = 6,
        CHANGE_LANG = 7,

        ON_PREPARE_FEEDBACK = 8,
        FEEDBACK = 9,

        MAIN_MENU = 11,
        SPECIALIZATION = 12,
        ORGANIZATION = 13,
        CATEGORY = 14,
        STUFF = 15,
        CHOOSE_DATE = 16,
        CHOOSE_TIME = 17,
        PHONE = 18,
        USERNAME = 19,
        ADD_FAVORITES = 20,
    }
}
