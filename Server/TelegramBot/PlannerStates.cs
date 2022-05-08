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

        GET_PINFL = 14,
        GET_SERIA = 15,

        CATEGORY = 16,
        STUFF = 17,
        CHOOSE_DATE = 18,
        CHOOSE_TIME = 19,
        PHONE = 20,
        USERNAME = 21,
        ADD_FAVORITES = 22,
    }
}
