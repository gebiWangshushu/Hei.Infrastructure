using Hei.Hystrix;
using Hei.Infrastructure;

namespace DemoApi.Net6.Services
{
    public interface IUserService
    {
        Task<string> MyFallback();

        /// <summary>
        /// 增加回退逻辑
        /// </summary>
        /// <returns></returns>
        [HeiHystrix(nameof(MyFallback))]
        Task OnlyFallback();

        /// <summary>
        /// 熔断处理
        /// </summary>
        /// <returns></returns>
        //[HeiHystrix(nameof(MyFallback), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, SecondsOfBreak = 10 * 1000)]
        //只有ArgumentException异常才熔断
        [HeiHystrix(OnError = typeof(ArgumentException), FallBackMethod = nameof(MyFallback), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, SecondsOfBreak = 10 * 1000)]
        //所有异常都熔断
        //  [HeiHystrix(nameof(MyFallback), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, SecondsOfBreak = 20 * 1000)]
        Task CircuitBreaker();

        /// <summary>
        /// 超时处理
        /// </summary>
        /// <returns></returns>
        //[HeiHystrix(nameof(MyFallback), TimeOutSeconds = 1*1000)]
        [HeiHystrix(nameof(MyFallback), TimeOutSeconds = 2 * 1000)]
        Task<string> TimeOut();

        /// <summary>
        /// 重试
        /// </summary>
        /// <returns></returns>
        //[HeiHystrix(nameof(Retry), MaxRetryTimes = 1, RetryIntervalSeconds = 0)]
        //所有异常都重试
        //[HeiHystrix(nameof(MyFallback), MaxRetryTimes = 2, RetryIntervalSeconds = 4 * 1000)]
        //只有ArgumentException异常才重试
        [HeiHystrix(OnError = typeof(ArgumentException), FallBackMethod = nameof(MyFallback), MaxRetryTimes = 1, RetryIntervalSeconds = 4 * 1000)] //只有ArgumentException异常才重试
        Task<string> Retry();

        Task<string> TaskRetry();

        /// <summary>
        /// 缓存
        /// </summary>
        /// <returns></returns>
        // [HeiHystrix(nameof(MyFallback), CacheTTLSeconds = 5)]//内存缓存，有fallback逻辑,缓存5秒
        // [HeiHystrix(CacheTTLMinutes = 2)] //内存缓存，缓存2分钟
        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]//缓存到redis，2分钟
        Task<List<string>> CacheDataAsync();

        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]
        List<string> CacheData();

        //缓存void测试
        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]
        void CacheVoid();

        //缓存task测试
        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]
        Task CacheTask();

        //字符串参数缓存
        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]
        Task<List<string>> CacheStringParam(string userId, int age);

        //对象参数缓存
        [HeiHystrix(CacheType = CacheTypeEnum.Redis, CacheTTLMinutes = 2)]
        Task<List<string>> CacheObjectParam(string userId, int age, HeiApiResult<object> apiResult);
    }
}