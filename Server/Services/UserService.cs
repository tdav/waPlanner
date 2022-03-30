﻿using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;

namespace waPlanner.Services
{
    public interface IUserService
    {
        Task InsertAsync(tbUser user);
        Task UpdateAsync(tbUser user);
        void Delete(tbUser user);
        void Delete(int id);
        Task<tbUser> GetUserByIdAsync(int id);
        Task<tbUser[]> GetAllAsync();
    }
    public class UserService : IUserService
    {
        private readonly MyDbContext myDb;
        public UserService(MyDbContext myDb)
        {
            this.myDb = myDb;
        }
        public async Task InsertAsync(tbUser user)
        {
            await myDb.tbUsers.AddAsync(user);
            await myDb.SaveChangesAsync();
        }
        public async Task UpdateAsync(tbUser user)
        {
            myDb.tbUsers.Update(user);
            await myDb.SaveChangesAsync();
        }
        public void Delete(tbUser user)
        {
            myDb.tbUsers.Remove(user);
            myDb.SaveChanges();
        }
        public void Delete(int id)
        {
            tbUser user = myDb.tbUsers.Find(id);
            myDb.tbUsers.Remove(user);
            myDb.SaveChanges();
        }
        public async Task<tbUser[]> GetAllAsync()
        {
            return await myDb.tbUsers.ToArrayAsync();
        }
        public async Task<tbUser> GetUserByIdAsync(int id)
        {
            return await myDb.tbUsers.FindAsync(id);
        }
    }
}
