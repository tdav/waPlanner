﻿using Arch.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using waPlanner.Database;
using waPlanner.ModelViews;

namespace waPlanner.TelegramBot.Utils
{
    public class DbManipulations
    {
        public static List<IdValue> GetStuffBySpec(MyDbContext db, string msg)
        {
            return db.tbUsers
                         .AsNoTracking()
                         .Include(i => i.Category)
                         .Where(x => x.UserTypeId == 1 && x.Category.NameUz == msg)
                         .Select(x => new IdValue { Id = x.Id, Name = $"{x.Surname} {x.Name} {x.Patronymic}" })
                         .ToList();
        }
        public static List<IdValue> GetAllCategories(MyDbContext db)
        {
            return db.spCategories.AsNoTracking().Select(x => new IdValue { Id = x.Id, Name = x.NameUz }).ToList();
        }
    }
}
