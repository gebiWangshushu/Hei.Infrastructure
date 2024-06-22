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
        //[HeiHystrix(nameof(MyFallback), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, MillisecondsOfBreak = 10 * 1000)]
        //只有ArgumentException异常才熔断
        [HeiHystrix<ArgumentException>(nameof(MyFallback), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, MillisecondsOfBreak = 10 * 1000)]
        Task CircuitBreaker();

        /// <summary>
        /// 超时处理
        /// </summary>
        /// <returns></returns>
        //[HeiHystrix(nameof(MyFallback), TimeOutMilliseconds = 1*1000)]
        [HeiHystrix(nameof(MyFallback), TimeOutMilliseconds = 2 * 1000)]
        Task<string> TimeOut();

        /// <summary>
        /// 重试
        /// </summary>
        /// <returns></returns>
        //[HeiHystrix(nameof(Retry), MaxRetryTimes = 1, RetryIntervalMilliseconds = 0)]
        //[HeiHystrix(nameof(MyFallback), MaxRetryTimes = 2, RetryIntervalMilliseconds = 4 * 1000)]
        [HeiHystrix<ArgumentException>(nameof(MyFallback), MaxRetryTimes = 1, RetryIntervalMilliseconds = 4 * 1000)] //只有ArgumentException异常才重试
        Task<string> Retry();

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

    public class UserService : IUserService
    {
        public async Task OnlyFallback()
        {
            Console.WriteLine("执行熔断方法 OnlyFallback");
            throw new Exception("fallback异常");
        }

        public async Task CircuitBreaker()
        {
            Console.WriteLine("执行熔断方法CircuitBreaker");
            throw new Exception("熔断异常");
        }

        public async Task<string> TimeOut()
        {
            Console.WriteLine("执行timeOut方法");
            await Task.Delay(2 * 1000);
            return "执行timeOut方法";
        }

        public async Task<string> Retry()
        {
            Console.WriteLine("执行方法Retry");
            //throw new Exception("重试异常");
            throw new ArgumentException("ArgumentException重试异常");

            return "执行方法Retry";
        }

        //public async Task<string> Retry()
        //{
        //    Console.WriteLine("执行方法Retry");
        //    throw new ArgumentException("重试异常");
        //    return "执行方法Retry";
        //}

        public void CacheVoid()
        {
            Console.WriteLine("执行缓存CacheVoid" + DateTime.Now.ToString());
        }

        public async Task CacheTask()
        {
            Console.WriteLine("执行缓存CacheVoid" + DateTime.Now.ToString());
        }

        public async Task<List<string>> CacheDataAsync()
        {
            var datatime = DateTime.Now.ToString();
            Console.WriteLine("执行缓存CacheData" + datatime);
            return new List<string>
            {
                datatime,
                new Random().Next(1,10000).ToString()
            };
        }

        public List<string> CacheData()
        {
            var datatime = DateTime.Now.ToString();
            Console.WriteLine("执行缓存CacheData" + datatime);
            return new List<string>
            {
                datatime,
                new Random().Next(1,10000).ToString()
            };
        }

        public async Task<List<string>> CacheStringParam(string userId, int age)
        {
            var datatime = DateTime.Now.ToString();
            Console.WriteLine("执行缓存CacheData" + datatime);
            return new List<string>
            {
                userId,
                datatime,
                new Random().Next(1,10000).ToString()
            };
        }

        public async Task<List<string>> CacheObjectParam(string userId, int age, HeiApiResult<object> apiResult)
        {
            var datatime = DateTime.Now.ToString();
            Console.WriteLine("执行缓存CacheData" + datatime);
            return new List<string>
            {
                userId,
                datatime,
                new Random().Next(1,10000).ToString()
            };
        }

        //ratelimit频率限制策略

        //Bulkhead壁仓隔离策略

        public async Task<string> MyFallback()
        {
            var msg = "MyFallback Executed!!!!!!!!!!!!!!!!!!";
            Console.WriteLine(msg);
            return msg;
        }
    }
}