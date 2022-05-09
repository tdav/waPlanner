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
        PREPARE_CHECK_PHONE = 3,
        CHECK_PHONE = 4,
        CHECK_PASSWORD = 5,

        SETTINGS = 6,
        PREPARE_CHANGE_NAME = 7,
        CHANGE_NAME = 8,
        CHANGE_PHONE = 9,
        CHANGE_LANG = 10,

        ON_PREPARE_FEEDBACK = 11,
        FEEDBACK = 12,

        MAIN_MENU = 13,
        SPECIALIZATION = 14,
        ORGANIZATION = 15,

        GET_PINFL = 16,
        GET_SERIA = 17,

        CATEGORY = 18,
        STUFF = 19,
        CHOOSE_DATE = 20,
        CHOOSE_TIME = 21,
        PHONE = 22,
        USERNAME = 23,
        ADD_FAVORITES = 24,
    }
}
