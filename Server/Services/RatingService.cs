﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Services
{
    public interface IRatingService
    {
        ValueTask<AnswerBasic> AddStaffRatingAsync(viRating rating);
        ValueTask<AnswerBasic> AddOrganizationRatingAsync(viRating rating);
        ValueTask<Answer<List<viRating>>> GetStaffRating(int staff_id);
        ValueTask<Answer<List<viRating>>> GetOrganizationRating(int organization_id);
    }
    public class RatingService : IRatingService, IAutoRegistrationScopedLifetimeService
    {
        private readonly MyDbContext db;
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly ILogger<CategoryService> logger;

        public RatingService(MyDbContext db, IHttpContextAccessorExtensions accessor, ILogger<CategoryService> logger)
        {
            this.db = db;
            this.accessor = accessor;
            this.logger = logger;
        }

        public async ValueTask<AnswerBasic> AddStaffRatingAsync(viRating rating)
        {
            try
            {
                int user_id = accessor.GetId();

                var userRate = await db.tbRatings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == rating.UserId && x.StaffId == rating.StaffId);

                if (userRate is not null) return new AnswerBasic(false, "Пользователь уже голосовал");

                var addRate = new tbRating
                {
                    UserId = rating.UserId,
                    StaffId = rating.StaffId,
                    OrganizationId = rating.OrganizationId,
                    Rating = rating.Rating,
                    Comment = rating.Comment,
                    Status = 1,
                    CreateDate = DateTime.Now,
                    CreateUser = user_id,
                };

                await db.AddRangeAsync(addRate);
                await db.SaveChangesAsync();

                var query = db.tbRatings.AsNoTracking().Where(x => x.StaffId == addRate.StaffId && x.OrganizationId == addRate.OrganizationId);

                var cnt = query.Count();

                if (cnt == 0) return new AnswerBasic(false, "Пользователь не найден");
                var sum = await query.SumAsync(x => x.Rating);

                var staff = await db.tbStaffs.FindAsync(addRate.StaffId);
                staff.Rating = sum / cnt;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception ex)
            {
                logger.LogError($"RatingService.AddStaffRatingAsync Error:{ex.Message} Model: {rating.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async ValueTask<AnswerBasic> AddOrganizationRatingAsync(viRating rating)
        {
            try
            {
                int user_id = accessor.GetId();

                var userRate = await db.tbRatings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == rating.UserId && x.OrganizationId == rating.OrganizationId && x.StaffId == null);

                if (userRate is not null) return new AnswerBasic(false, "Пользователь уже голосовал");

                var addRate = new tbRating
                {
                    UserId = rating.UserId,
                    OrganizationId = rating.OrganizationId,
                    Rating = rating.Rating,
                    Comment = rating.Comment,
                    Status = 1,
                    CreateDate = DateTime.Now,
                    CreateUser = user_id,
                };

                await db.AddRangeAsync(addRate);
                await db.SaveChangesAsync();

                var query = db.tbRatings.AsNoTracking().Where(x => x.StaffId == null && x.OrganizationId == addRate.OrganizationId);

                var cnt = query.Count();

                if (cnt == 0) return new AnswerBasic(false, "Пользователь не найден");
                var sum = await query.SumAsync(x => x.Rating);

                var organization = await db.spOrganizations.FindAsync(addRate.OrganizationId);
                organization.Rating = sum / cnt;
                await db.SaveChangesAsync();

                return new AnswerBasic(true, "");
            }
            catch (Exception ex)
            {
                logger.LogError($"RatingService.AddOrganizationRatingAsync Error:{ex.Message} Model: {rating.ToJson()}");
                return new AnswerBasic(false, "Ошибка программы");
            }
        }

        public async ValueTask<Answer<List<viRating>>> GetStaffRating(int staff_id)
        {
            try
            {
                var rating = await db.tbRatings
                .AsNoTracking()
                .Where(x => x.Status == 1 && x.StaffId == staff_id)
                .Select(x => new viRating
                {
                    StaffId = x.StaffId,
                    Rating = x.Rating,
                    UserId = x.UserId,
                    OrganizationId = x.OrganizationId,
                    Comment = x.Comment,
                })
                .Take(50)
                .ToListAsync();

                return new Answer<List<viRating>>(true, "", rating);
            }
            catch (Exception ex)
            {
                logger.LogError($"RatingService.GetStaffRating Error:{ex.Message}");
                return new Answer<List<viRating>>(false, "Ошибка программы", null);
            }
        }

        public async ValueTask<Answer<List<viRating>>> GetOrganizationRating(int organization_id)
        {
            try
            {
                var rating = await db.tbRatings
                .AsNoTracking()
                .Where(x => x.Status == 1 && x.OrganizationId == organization_id)
                .Select(x => new viRating
                {
                    Rating = x.Rating,
                    UserId = x.UserId,
                    OrganizationId = x.OrganizationId,
                    Comment = x.Comment,
                })
                .Take(50)
                .ToListAsync();

                return new Answer<List<viRating>>(true, "", rating);
            }
            catch (Exception ex)
            {
                logger.LogError($"RatingService.GetOrganizationRating Error:{ex.Message}");
                return new Answer<List<viRating>>(false, "Ошибка программы", null);
            }
        }
    }
}
