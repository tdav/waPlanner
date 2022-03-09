//using Arch.EntityFrameworkCore.UnitOfWork;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using waPlanner.ModelViews;

//namespace waPlanner.Extensions
//{
//    [ApiController]
//    [ApiVersion("1.0")]
//    [Route("api/[controller]")]
//    public class BaseController<T> : ControllerBase where T : class, IBaseModel
//    {
//        private readonly IUnitOfWork uow;
//        private readonly IMemoryCache cache;

//        public BaseController(IUnitOfWork unitOfWork, IMemoryCache _cache)
//        {
//            uow = unitOfWork;
//            cache = _cache;
//        }

//        [HttpGet]
//        public virtual async Task<ActionResult<IList<T>>> Get()
//        {
//            if (cache.TryGetValue(typeof(T).FullName, out IList<T> value))
//            {
//                return Ok(value);
//            }
//            else
//            {
//                var _storage = uow.GetRepository<T>();
//                var res = await _storage.GetAllAsync(x => x.Status == 1);

//                if (res == null) return NotFound();

//                var cacheEntryOptions = new MemoryCacheEntryOptions();
//                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(5));

//                cache.Set(typeof(T).FullName, res, cacheEntryOptions);
//                return Ok(res);
//            }
//        }

//        [HttpGet("{id}")]
//        public virtual async Task<ActionResult<T>> Get(int id)
//        {
//            if (cache.TryGetValue(typeof(T).FullName + id.ToString(), out T value))
//            {
//                return value;
//            }
//            else
//            {
//                var _storage = uow.GetRepository<T>();
//                var res = await _storage.FindAsync(id);

//                if (res == null) return NotFound();

//                var cacheEntryOptions = new MemoryCacheEntryOptions();
//                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(3));
//                cache.Set(typeof(T).FullName + id.ToString(), res, cacheEntryOptions);
//                return Ok(res);
//            }
//        }

//        [HttpPost]
//        public virtual async Task<ActionResult<T>> Post([FromBody] T value)
//        {
//            var _storage = uow.GetRepository<T>();
//            await _storage.InsertAsync(value);
//            await uow.SaveChangesAsync();
//            return Ok(value);
//        }

//        [HttpPut]
//        public virtual async Task<ActionResult> Put([FromBody] T value)
//        {
//            var _storage = uow.GetRepository<T>();
//            _storage.Update(value);
//            await uow.SaveChangesAsync();
//            return Ok();
//        }

//        [HttpDelete]
//        public virtual async Task<ActionResult> Delete(int id)
//        {
//            var _storage = uow.GetRepository<T>();
//            _storage.Delete(id);
//            await uow.SaveChangesAsync();
//            return Ok();
//        }

//        public bool IsRoleAdmin
//        {
//            get
//            {
//                var role = User.FindFirst(ClaimTypes.Role);
//                return role.Value.Contains("1;");
//            }
//        }

//        public int GetId
//        {
//            get
//            {
//                var r = User?.FindFirst(ClaimTypes.Sid);
//                return r == null ? 0 : Convert.ToInt32(r.Value);
//            }
//        }

//        public string GetAccess
//        {
//            get
//            {
//                var role = User.FindFirst(ClaimTypes.Role);
//                return role.Value;
//            }
//        }

//        public string GetUserFullName
//        {
//            get
//            {
//                var role = User.FindFirst(ClaimTypes.Name);
//                return role.Value;
//            }
//        }

//        public string GetUserPhone
//        {
//            get
//            {
//                var role = User.FindFirst(ClaimTypes.MobilePhone);
//                return role.Value;
//            }
//        }

//        public string GetUserIp
//        {
//            get
//            {
//                return Request.HttpContext.Connection.RemoteIpAddress.ToString();
//            }
//        }
//    }
//}


