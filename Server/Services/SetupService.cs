using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Extensions;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface ISetupService
    {
        Task<viSetup> Get();
        Task Set(viSetup v);
    }

    public class SetupService : ISetupService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;

        public SetupService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.db = db;
            this.accessor = accessor;
        }
        public async Task<viSetup> Get()
        {
            int userId = accessor.GetId();
            var s = await db.spSetups.AsNoTracking().FirstOrDefaultAsync(x => x.StaffId == userId);
            return viSetup.FromJson(s.Text);
        }

        public async Task Set(viSetup v)
        {
            int userId = accessor.GetId();
            var s = await db.spSetups.FindAsync(userId);
            
            if (s == null)
            {
                s = new Database.Models.spSetup();
                s.Status = 1;
                s.CreateDate = System.DateTime.Now;
                s.StaffId = userId;
            }
            else
            {
                s.UpdateUser = userId;
                s.UpdateDate = System.DateTime.Now;
            }

            s.Text = v.ToString();
            await db.spSetups.AddAsync(s);
            await db.SaveChangesAsync();
        }
    }
}
