using DemoApi.Net6;
using Hei.Hystrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.UnitTest
{
    [TestClass]
    public class OrderServiceTest : BaseTest
    {
        [TestMethod]
        public async Task TestRetry1()
        {
            var retryCount = 3;

            try
            {
                //任何异常都重试
                // await HeiHystrix.Retry(() => orderService.Retry(), retryCount, 5);

                //只重试ArgumentException
                await HeiHystrix.Retry(() => orderService.Retry(), retryCount, 5, typeof(ArgumentException));
            }
            catch (Exception)
            {
                throw;
            }

            if (TestAssist.TestValue < retryCount)
            {
                Assert.Fail();
            }
        }
    }
}