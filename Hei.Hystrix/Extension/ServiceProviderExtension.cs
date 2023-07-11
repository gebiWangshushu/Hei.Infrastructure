using AspectCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hei.Hystrix.Extension
{
    public static class ServiceProviderExtension
    {
        public static JsonSerializerOptions JsonSerializerOptions(this IServiceProvider serviceProvider)
        {
            var options = serviceProvider?.Resolve<IOptions<HeiHystrixOptions>>();
            if (options?.Value?.JsonSerializerOptions != null)
            {
                return options?.Value?.JsonSerializerOptions;
            }
            else
            {
                return new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    MaxDepth = 64,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
            }
        }
    }
}