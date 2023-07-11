using Hei.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace Hei.Hystrix
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddHeiHystrix(this IServiceCollection services, Action<HeiHystrixOptions> action = null)
        {
            if (action != null)
            {
                var options = new HeiHystrixOptions();
                action(options);

                if (options.RedisConnectionString.IsNotNullOrEmpty())
                {
                    var redis = services.BuildServiceProvider().GetService<IDatabase>();
                    if (redis == null)
                    {
                        services.AddRedis(options.RedisConnectionString);
                    }
                }
                if (options.JsonSerializerOptions == null)
                {
                    options.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        MaxDepth = 64,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    };
                }
                services.Configure<HeiHystrixOptions>(o =>
                {
                    o.JsonSerializerOptions = options.JsonSerializerOptions;
                    o.RedisConnectionString = options.RedisConnectionString;
                });
            }

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.ConfigureDynamicProxy();
            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(connectionString);
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddScoped<IDatabase>(c => redisConnection.GetDatabase());

            return services;
        }
    }
}