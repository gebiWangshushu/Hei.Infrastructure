using Hei.Hystrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.UnitTest
{
    [TestClass]
    public class UserServiceTest : BaseTest
    {
        [TestMethod]
        public async Task TestRetry1()
        {
            //HeiHystrix.Retry(() => userService.get);
        }
    }
}