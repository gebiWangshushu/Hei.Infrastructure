namespace DemoApi.Net6.Services
{
    public class OrderService : IOrderService
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

        public async Task<string> Retry()
        {
            TestAssist.TestValue++;

            Console.WriteLine("执行方法Retry");
            //throw new Exception("重试异常");
            throw new ArgumentException("ArgumentException重试异常");

            return "执行方法Retry";
        }

        public async Task<string> MyFallback()
        {
            var msg = "执行我的fallback方法：MyFallback Executed!!!!!!!!!!!!!!!!!!";
            Console.WriteLine(msg);
            return msg;
        }
    }
}