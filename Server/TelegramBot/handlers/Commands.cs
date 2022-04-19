using System.Collections.Generic;

namespace waPlanner.TelegramBot.handlers
{
    public struct Commands
    {
        public static List<string> back = new List<string> { "⬅️Назад", "⬅️Ortga", "⬅️Ортга" }; 
        public static List<string> book = new List<string> { "Сделать бронь📄", "Band qilish📄", "Банд килиш📄" }; 
        public static List<string> favorites = new List<string> { "Избранное🌟", "Sevimlilar🌟", "Севимлилиар🌟"}; 
        public static List<string> settings = new List<string> { "Настройки🛠", "So'zlamalar🛠", "Сўзламалар🛠" }; 
        public static List<string> feedback = new List<string> { "Оставить отзыв💌", "Izoh qoldirish💌", "Изоҳ қолдириш💌" }; 
        public static List<string> about_us = new List<string> { "О нас💬", "Biz haqimizda💬", "Биз ҳақимизда" }; 
        public static List<string> contacts = new List<string> { "Контакты📱", "Kontaktlar📱", "Контактлар📱" }; 
        public static List<string> change_name = new List<string> { "Изменить имя", "Ismni o'zgartirish", "Исимни ўзгартириш" }; 
        public static List<string> change_phone = new List<string> { "Изменить номер☎️", "Nomerni o'zgartirish☎️", "Номерни ўзгартириш☎️" }; 
        public static List<string> change_lang = new List<string> { "Изменить язык🇷🇺", "Tilni o'zgartirish🇺🇿", "Тилни ўзгартириш🇺🇿" }; 
    }
}
