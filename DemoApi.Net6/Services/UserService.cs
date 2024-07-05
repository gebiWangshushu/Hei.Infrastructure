using Hei.Hystrix;
using Hei.Infrastructure;

namespace DemoApi.Net6.Services
{
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
            //throw new Exception("熔断异常Exception");
            throw new ArgumentException("熔断异常ArgumentException");
        }

        public async Task<string> TimeOut()
        {
            Console.WriteLine("执行timeOut方法");
            await Task.Delay(2 * 1000);
            return "执行timeOut方法";
        }

        public async Task<string> TaskRetry()
        {
            Console.WriteLine("执行方法Retry");
            throw new Exception("重试异常");
            // throw new ArgumentException("ArgumentException重试异常");

            return "执行方法Retry";
        }

        public async Task<string> Retry()
        {
            Console.WriteLine("执行方法Retry");
            throw new Exception("重试异常");
            // throw new ArgumentException("ArgumentException重试异常");


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
            var msg = "执行我的fallback方法：MyFallback Executed!!!!!!!!!!!!!!!!!!";
            Console.WriteLine(msg);
            return msg;
        }
    }
}