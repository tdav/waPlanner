﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface ISpecializationService
    {
        Task<Answer<viSpecialization>> AddSpecializationAsync(viSpecialization spec);
        Task<Answer<viSpecialization>> UpdateSpecializationAsync(viSpecialization spec);
        Task<Answer<List<spSpecialization>>> GetSpecializationsAsync();
        Task<Answer<spSpecialization>> GetSpecializationByIdAsync(int spec_id);
        Task<AnswerBasic> ChangeSpecializationStatus(viSetStatus status);
    }
    public class SpecializationService : ISpecializationService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<SpecializationService> logger;
        public SpecializationService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<SpecializationService> logger)
        {
            this.db = db;
            this.accessor = accessor;
            this.logger = logger;
        }

        public async Task<Answer<viSpecialization>> AddSpecializationAsync(viSpecialization spec)
        {
            try
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
                return new Answer<viSpecialization>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"SpecializationService.AddSpecializationAsync Error:{e.Message} Model: {spec.ToJson()}");
                return new Answer<viSpecialization>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<viSpecialization>> UpdateSpecializationAsync(viSpecialization spec)
        {
            try
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
                return new Answer<viSpecialization>(true, "", null);
            }
            catch (Exception e)
            {
                logger.LogError($"SpecializationService.UpdateSpecializationAsync Error:{e.Message} Model: {spec.ToJson()}");
                return new Answer<viSpecialization>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<List<spSpecialization>>> GetSpecializationsAsync()
        {
            try
            {
                var spec = await db.spSpecializations
                .AsNoTracking()
                .ToListAsync();
                return new Answer<List<spSpecialization>>(true, "", spec);
            }
            catch (Exception e)
            {
                logger.LogError($"SpecializationService.GetSpecializationsAsync Error:{e.Message}");
                return new Answer<List<spSpecialization>>(false, "Ошибка программы", null);
            }

        }

        public async Task<Answer<spSpecialization>> GetSpecializationByIdAsync(int spec_id)
        {
            try
            {
                var spec = await db.spSpecializations
                                .AsNoTracking()
                                .FirstAsync(x => x.Id == spec_id);
                return new Answer<spSpecialization>(true, "", spec);
            }
            catch (Exception e)
            {
                logger.LogError($"SpecializationService.GetSpecializationByIdAsync Error:{e.Message}");
                return new Answer<spSpecialization>(false, "Ошибка программы", null);
            }

        }

        public async Task<AnswerBasic> ChangeSpecializationStatus(viSetStatus status)
        {
            try
            {
                int user_id = accessor.GetId();
                var specialization = await db.spSpecializations.FindAsync(status.Id);
                specialization.UpdateDate = DateTime.Now;
                specialization.UpdateUser = user_id;
                specialization.Status = status.Status;
                await db.SaveChangesAsync();
                return new AnswerBasic(true, "");
            }
            catch (Exception e)
            {
                logger.LogError($"SpecializationService.ChangeSpecializationStatus Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }
    }
}
