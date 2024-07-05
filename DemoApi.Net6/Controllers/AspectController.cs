using Microsoft.AspNetCore.Mvc;
using Hei.Infrastructure;
using DemoApi.Net6.Services;
using Microsoft.Extensions.Caching.Memory;
using Hei.Hystrix;

namespace DemoApi.Net6.Controllers
{
    [Route("aspect/[action]")]
    public class AspectController : HeiApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ICustomService _service;
        private readonly IMemoryCache _memoryCache;

        public AspectController(IConfiguration configuration, IUserService userService, ICustomService service, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _userService = userService;
            _service = service;
            _memoryCache = memoryCache;
        }

        //[HttpGet]
        //public async Task<IActionResult> Call()
        //{
        //    _service.Call();

        //    return Success("success");
        //}

        [HttpGet]
        public async Task<IActionResult> OnlyFallback()
        {
            await _userService.OnlyFallback();
            return Success("»ØÍË");
        }

        [HttpGet]
        public async Task<IActionResult> CircuitBreaker()
        {
            await _userService.CircuitBreaker();
            return Success("¶ÏÂ·Æ÷");
        }

        [HttpGet]
        public async Task<IActionResult> TimeOut()
        {
            var result = await _userService.TimeOut();
            return Success(result);
        }

        [HttpGet]
        public async Task<IActionResult> Retry()
        {
            await _userService.Retry();

            ////await _userService.TaskRetry().RetryV1(5, 3000, typeof(Exception));

            //var func = () => _userService.TaskRetry();
            //await func.RetryV2(5, 3000, typeof(ArgumentException));

            return Success("Retry");
        }

        [HttpGet]
        public async Task<IActionResult> CacheNone()
        {
            //_userService.CacheVoid();
            _userService.CacheTask();

            return Success("CacheData");
        }

        [HttpGet]
        public async Task<IActionResult> CacheData()
        {
            var data = _userService.CacheData();

            return Success("CacheData", data);
        }

        [HttpGet]
        public async Task<IActionResult> CacheData2()
        {
            var data = await _userService.CacheDataAsync();

            return Success("CacheDataAsync", data);
        }

        [HttpGet]
        public async Task<IActionResult> CacheParam()
        {
            var data = await _userService.CacheStringParam("wangnima", 18);
            var data2 = await _userService.CacheObjectParam("wangnima", 18, new HeiApiResult<object>() { Message = "CacheParam" });

            return Success("CacheData", new { data, data2 });
        }
    }
}