using Microsoft.OpenApi.Models;
using Hei.Infrastructure;
using System.Reflection;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Unchase.Swashbuckle.AspNetCore.Extensions.Options;

namespace DemoApi.Net6
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                //文档
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "『xxx』后端接口:cutapi",
                    Version = "v1",
                    Description = "（请求参数详细说明点击[Example Value]旁边的【Model】)",
                });
                c.MapType<long>(() => new OpenApiSchema { Type = "string" });

                //获取注释
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                var modXmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(HeiApiResult<object>).GetTypeInfo().Assembly.GetName().Name}.xml");
                c.IncludeXmlComments(apiXmlPath);
                if (File.Exists(modXmlPath))
                {
                    c.IncludeXmlComments(modXmlPath);
                }

                //c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.AddEnumsWithValuesFixFilters(o =>
                {
                    o.IncludeDescriptions = true;
                    o.IncludeXEnumRemarks = true;
                    o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
                    o.IncludeXmlCommentsFrom(modXmlPath);
                    o.IncludeXmlCommentsFrom(apiXmlPath);
                });
            });

            return services;
        }
    }
}