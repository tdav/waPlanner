using Microsoft.EntityFrameworkCore;

namespace waPlanner.Database.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<spAccess>().HasData(
                               new spAccess
                               {
                                   Id = 1,
                                   NameLt = "Create",
                                   NameRu = "Create",
                                   NameUz = "Create",                                   
                                   Status = 1,
                                   CreateDate = DateTime.UtcNow,
                                   CreateUser = 1
                               },
                               new spAccess
                               {
                                   Id = 2,
                                   NameLt = "Delete",
                                   NameRu = "Delete",
                                   NameUz = "Delete",
                                   Status = 1,
                                   CreateDate = DateTime.UtcNow,
                                   CreateUser = 1
                               },
                               new spAccess
                               {
                                   Id = 3,
                                   NameLt = "Update",
                                   NameRu = "Update",
                                   NameUz = "Update",
                                   Status = 1,
                                   CreateDate = DateTime.UtcNow,
                                   CreateUser = 1
                               },
                               new spAccess
                               {
                                   Id = 4,
                                   NameLt = "View",
                                   NameRu = "View",
                                   NameUz = "View",
                                   Status = 1,
                                   CreateDate = DateTime.UtcNow,
                                   CreateUser = 1
                               });

//            modelBuilder.Entity<spRole>().HasData(
//                    new spRole
//                    {
//                        Id = 1,
//                        NameLt = "admin",
//                        NameRu = "admin",
//                        NameUz = "admin",
//                        Status = 1,
//                        CreateDate = DateTime.UtcNow,
//                        CreateUser = 1
//                    },
//                    new spRole
//                    {
//                        Id = 2,
//                        NameLt = "user",
//                        NameRu = "user",
//                        NameUz = "user",
//                        Status = 1,
//                        CreateDate = DateTime.UtcNow,
//                        CreateUser = 1
//                    });

            modelBuilder.Entity<tbUser>().HasData(
                    new tbUser
                    {
                        Id = 1,
                        Surname = "admin",
                        Name = "admin",
                        Patronymic = "admin",
                        Login="admin",
                        Password = CHash.EncryptMD5("1"),
                        PhoneNum="998998278960",
                        LastSesion=DateTime.Now,                        
                        Status = 1,
                        CreateDate = DateTime.UtcNow,
                        CreateUser = 1
                    },
                    new tbUser
                    {
                        Id = 2,
                        Surname = "guest",
                        Name = "guest",
                        Patronymic = "guest",
                        Login = "guest",
                        Password = CHash.EncryptMD5("1"),
                        PhoneNum = "998998278960",
                        LastSesion = DateTime.Now,
                        Status = 1,
                        CreateDate = DateTime.UtcNow,
                        CreateUser = 1
                    });
            */
        }
    }
}
