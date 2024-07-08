using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using DemoApi.Net6.Services;
using Microsoft.Extensions.Logging;
using DemoApi.Net6;
using Microsoft.AspNetCore.Hosting;

namespace DemoApi.UnitTest
{
    public class BaseTest
    {
        protected readonly IServiceProvider serviceProvider;
        protected HttpClient httpClient;
        protected IUserService userService;
        protected IOrderService orderService;
        protected ILogger<BaseTest> logger;

        public BaseTest()
        {
            var app = new WebApplicationFactory<Program>()

                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment(EnvironmentName.Development);

                    //builder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                });
            var scope = app.Services.CreateScope();
            serviceProvider = scope.ServiceProvider;
            httpClient = app.CreateClient();
            userService = serviceProvider.GetRequiredService<IUserService>();
            orderService = serviceProvider.GetRequiredService<IOrderService>();
            logger = serviceProvider.GetRequiredService<ILogger<BaseTest>>();
        }
    }
}