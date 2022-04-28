using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface ISpecializationService
    {
        Task AddSpecializationAsync(viSpecialization spec);
        Task UpdateSpecializationAsync(viSpecialization spec);
        Task<spSpecialization[]> GetSpecializationsAsync();
        Task<spSpecialization> GetSpecializationByIdAsync(int spec_id);
        Task ChangeSpecializationStatus(int spec_id, int status);
    }
    public class SpecializationService: ISpecializationService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        public SpecializationService(MyDbContext db, IHttpContextAccessorExtensions accessor)
        {
            this.db = db;
            this.accessor = accessor;
        }

        public async Task AddSpecializationAsync(viSpecialization spec)
        {
            int user_id = accessor.GetId();
            var specialization = new spSpecialization
            {
                NameLt = spec.NameLt,
                NameRu = spec.NameRu,
                NameUz = spec.NameUz,
                Status = 1,
                CreateDate = DateTime.Now,
                CreateUser = user_id
            };

            await db.spSpecializations.AddAsync(specialization);
            await db.SaveChangesAsync();
        }

        public async Task UpdateSpecializationAsync(viSpecialization spec)
        {
            int user_id = accessor.GetId();
            var specialization = await db.spSpecializations.FindAsync(spec.Id);

            specialization.NameLt = spec.NameLt;
            specialization.NameRu = spec.NameRu;
            specialization.NameUz = spec.NameUz;
            specialization.Status = spec.Status;
            specialization.UpdateDate = DateTime.Now;
            specialization.UpdateUser = user_id;

            await db.SaveChangesAsync();
        }

        public async Task<spSpecialization[]> GetSpecializationsAsync()
        {
            return await db.spSpecializations
                .AsNoTracking()
                .ToArrayAsync();
        }

        public async Task<spSpecialization> GetSpecializationByIdAsync(int spec_id)
        {
            return await db.spSpecializations
                .AsNoTracking()
                .FirstAsync(x => x.Id == spec_id);
        }

        public async Task ChangeSpecializationStatus(int spec_id, int status)
        {
            int user_id = accessor.GetId();
            var specialization = await db.spSpecializations.FindAsync(spec_id);
            specialization.UpdateDate = DateTime.Now;
            specialization.UpdateUser = user_id;
            specialization.Status = status;
            await db.SaveChangesAsync();
        }
    }
}
