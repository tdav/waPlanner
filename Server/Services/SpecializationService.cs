﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.ModelViews;

namespace waPlanner.Services
{
    public interface ISpecializationService
    {
        Task AddSpecializationAsync(viSpecialization spec);
        Task UpdateSpecializationAsync(viSpecialization spec);
        Task<spSpecialization[]> GetSpecializationsAsync();
        Task<spSpecialization> GetSpecializationByIdAsync(int spec_id);
        Task ChangeSpecializationStatus(viSpecialization spec, int status);
    }
    public class SpecializationService: ISpecializationService
    {
        private readonly MyDbContext db;
        public SpecializationService(MyDbContext db)
        {
            this.db = db;
        }

        public async Task AddSpecializationAsync(viSpecialization spec)
        {
            var specialization = new spSpecialization
            {
                NameLt = spec.NameLt,
                NameRu = spec.NameRu,
                NameUz = spec.NameUz,
                Status = spec.Status,
                CreateDate = DateTime.Now,
                CreateUser = 1
            };

            await db.spSpecializations.AddAsync(specialization);
            await db.SaveChangesAsync();
        }

        public async Task UpdateSpecializationAsync(viSpecialization spec)
        {
            var specialization = await db.spSpecializations.FindAsync(spec.Id);

            specialization.NameLt = spec.NameLt;
            specialization.NameRu = spec.NameRu;
            specialization.NameUz = spec.NameUz;
            specialization.Status = spec.Status;
            specialization.UpdateDate = DateTime.Now;
            specialization.UpdateUser = 1;

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

        public async Task ChangeSpecializationStatus(viSpecialization spec, int status)
        {
            var specialization = await db.spSpecializations.FindAsync(spec.Id);
            specialization.UpdateDate = DateTime.Now;
            specialization.UpdateUser = 1;
            specialization.Status = status;
            await db.SaveChangesAsync();
        }
    }
}
