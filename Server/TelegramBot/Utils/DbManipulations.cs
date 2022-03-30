using Arch.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using waPlanner.Database;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.Utils
{
    public class DbManipulations
    {
        public static List<IdValue> GetStuffByCategory(MyDbContext db, string category)
        {
            return db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToList();
        }
        public static bool CheckStuffByCategory(MyDbContext db, string category, string value)
        {
            var list = db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == category)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToList();
            return list.Any(x => x.Name == value);
        }
        public static List<IdValue> GetAllCategories(MyDbContext db)
        {
            return db.spCategories.AsNoTracking().Select(x => new IdValue { Id = x.Id, Name = x.NameUz }).ToList();
        }

    }
}
