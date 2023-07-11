using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hei.Infrastructure
{
    public static class Bootstrapper
    {
        /// <summary>
        ///  统一处理一下 模型验证失败的响应
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var firstInvalidMsg = actionContext.ModelState?.Values.SelectMany(c => c.Errors).Select(c => c.ErrorMessage)?.FirstOrDefault();

                    return new JsonResult(new HeiApiResult<object>()
                    {
                        Code = EnumStatus.Fail,
                        Message = firstInvalidMsg ?? "参数验证失败"
                    });
                };
            });
            return services;
        }
    }
}