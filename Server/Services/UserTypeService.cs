//using Arch.EntityFrameworkCore.UnitOfWork;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Service.Interfaces;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using waPlanner.Database.Models;
//using waPlanner.ModelViews;

//namespace waPlanner.Services
//{
//    public class UserTypeService : IBaseSerive<spUserType>, IAsyncDisposable
//    {
//        private readonly IUnitOfWork uow;
//        private readonly IMemoryCache cache;
//        private readonly IRepository<spUserType> storage;
//        private readonly ILogger<UserTypeService> logger;
//        private readonly Vars vars;

//        public UserTypeService(IUnitOfWork uow, IMemoryCache cache, ILogger<UserTypeService> logger, IOptions<Vars> options)
//        {
//            this.uow = uow;
//            this.cache = cache;
//            this.logger = logger;

//            vars = options.Value;
//            storage = this.uow.GetRepository<spUserType>();
//        }

//        public async ValueTask DisposeAsync()
//        {
//            try
//            {
//                await uow.GetDbConnection().CloseAsync();
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("DisposeAsync Error:{@error}", ee.GetAllMessages());
//            }
//            uow.Dispose();
//        }

//        public async ValueTask<Answer<spUserType[]>> Get()
//        {
//            try
//            {
//                if (cache.TryGetValue("spUserType", out spUserType[] value))
//                {
//                    return new Answer<spUserType[]>(true, "", value);
//                }
//                else
//                {
//                    var res = await storage.GetAllAsync(x => x.Status == 1);

//                    if (res == null) return new Answer<spUserType[]>(false, "Маълумот топилмади", null);

//                    var cacheEntryOptions = new MemoryCacheEntryOptions();
//                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(vars.CacheTimeOut));

//                    cache.Set("spUserType", res, cacheEntryOptions);
//                    return new Answer<spUserType[]>(true, "", res.ToArray());
//                }
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("Get Error:{@error}", ee.GetAllMessages());
//                return new Answer<spUserType[]>(false, "Тизимда хато", null);
//            }
//        }

//        public virtual async ValueTask<Answer<spUserType>> Get(int id)
//        {
//            try
//            {
//                if (cache.TryGetValue($"spUserType_{id}", out spUserType value))
//                {
//                    return new Answer<spUserType>(true, "", value);
//                }
//                else
//                {
//                    var res = await storage.FindAsync(id);

//                    if (res == null) return new Answer<spUserType>(false, "Маълумот топилмади", null);

//                    var cacheEntryOptions = new MemoryCacheEntryOptions();
//                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(vars.CacheTimeOut));
//                    cache.Set($"spUserType_{id}", res, cacheEntryOptions);
//                    return new Answer<spUserType>(true, "", res);
//                }
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("Get Id:{@id} Error:{@error}", id, ee.GetAllMessages());
//                return new Answer<spUserType>(false, "Тизимда хато", null);
//            }
//        }

//        public async ValueTask<Answer<spUserType>> Post(spUserType value)
//        {
//            try
//            {
//                await storage.InsertAsync(value);
//                await uow.SaveChangesAsync();
//                return new Answer<spUserType>(true, "", value);
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("Post Error:{@error}", ee.GetAllMessages());
//                return new Answer<spUserType>(false, "Тизимда хато", null);
//            }
//        }

//        public async ValueTask<AnswerBasic> Put(spUserType value)
//        {
//            try
//            {
//                storage.Update(value);
//                await uow.SaveChangesAsync();
//                return new AnswerBasic(true, "");
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("Put Error:{@error}", ee.GetAllMessages());
//                return new AnswerBasic(false, "Тизимда хато");
//            }
//        }

//        public async ValueTask<AnswerBasic> Delete(int id)
//        {
//            try
//            {
//                storage.Delete(id);
//                await uow.SaveChangesAsync();
//                return new AnswerBasic(true, "");
//            }
//            catch (Exception ee)
//            {
//                logger.LogError("Delete Error:{@error}", ee.GetAllMessages());
//                return new AnswerBasic(false, "Тизимда хато");
//            }
//        }
//    }
//}
