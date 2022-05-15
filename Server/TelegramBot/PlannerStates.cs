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
        SELECT_STAFF_FAVORITES = 2,
        SEND_STAFF_FAVORITES = 3,

        SELECT_ORG_FAVORITES = 4,
        SEND_ORG_FAVORITES = 5,
        
        PREPARE_CHECK_PHONE = 6,
        CHECK_PHONE = 7,
        CHECK_PASSWORD = 8,

        SETTINGS = 9,
        PREPARE_CHANGE_NAME = 10,
        CHANGE_NAME = 11,
        CHANGE_PHONE = 12,
        CHANGE_LANG = 13,

        ON_PREPARE_FEEDBACK = 14,
        FEEDBACK = 15,

        MAIN_MENU = 16,
        SPECIALIZATION = 17,
        ORGANIZATION = 18,

        GET_PINFL = 19,
        GET_SERIA = 20,

        CATEGORY = 21,
        STAFF = 22,
        CHOOSE_DATE = 23,
        CHOOSE_TIME = 24,
        PHONE = 25,
        USERNAME = 26,
        ADD_FAVORITES = 27,
        ANALYSIS = 28,
    }
}
