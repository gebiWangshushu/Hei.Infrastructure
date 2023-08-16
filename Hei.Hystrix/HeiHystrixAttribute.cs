using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Hei.Hystrix.Extension;
using Hei.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

//using Newtonsoft.Json;
using Polly;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;

namespace Hei.Hystrix
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HeiHystrixAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 最多重试几次，如果为0则不重试
        /// </summary>
        public int MaxRetryTimes { get; set; } = 0;

        /// <summary>
        /// 重试间隔的毫秒数
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 100;

        /// <summary>
        /// 是否启用熔断
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = false;

        /// <summary>
        /// 熔断前出现允许错误几次(默认3)
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;

        /// <summary>
        /// 熔断多长时间（毫秒）
        /// </summary>
        public int MillisecondsOfBreak { get; set; } = 1000;

        /// <summary>
        /// 执行超过多少毫秒则认为超时（0表示不检测超时）
        /// </summary>
        public int TimeOutMilliseconds { get; set; } = 0;

        /// <summary>
        /// 降级方法名
        /// </summary>
        public string FallBackMethod { get; set; }

        /// <summary>
        /// 缓存类型，支持redis和内存缓存;默认内存缓存
        /// </summary>
        public CacheTypeEnum CacheType { get; set; } = CacheTypeEnum.MemoryCache;

        /// <summary>
        /// 缓存多少秒（0表示不缓存），用“类名+方法名+所有参数ToString拼接”做缓存Key
        /// </summary>
        public int CacheTTLSeconds { get; set; } = 0;

        /// <summary>
        /// 缓存多少分钟(优先级比<CacheTTLSeconds>高)
        /// </summary>
        public int CacheTTLMinutes { get; set; } = 0;

        /// <summary>
        /// 是否全局缓存：是的话所有用户公用一个key
        /// </summary>
        public bool CacheGlobal { get; set; } = false;

        private static ConcurrentDictionary<MethodInfo, AsyncPolicy> policies = new ConcurrentDictionary<MethodInfo, AsyncPolicy>();

        //private static readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        private IMemoryCache _memoryCache;

        /// <summary>
        ///
        /// </summary>
        /// <param name="fallBackMethod">降级的方法名</param>
        public HeiHystrixAttribute(string fallBackMethod = null)
        {
            this.FallBackMethod = fallBackMethod;
        }

        /// <summary>
        /// 本思路由杨老师原创
        /// 请参考： https://github.com/yangzhongke/RuPeng.HystrixCore
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            //一个HystrixCommand中保持一个policy对象即可
            //其实主要是CircuitBreaker要求对于同一段代码要共享一个policy对象
            //根据反射原理，同一个方法的MethodInfo是同一个对象，但是对象上取出来的HystrixCommandAttribute
            //每次获取的都是不同的对象，因此以MethodInfo为Key保存到policies中，确保一个方法对应一个policy实例
            policies.TryGetValue(context.ServiceMethod, out AsyncPolicy policy);
            lock (policies)//因为Invoke可能是并发调用，因此要确保policies赋值的线程安全
            {
                if (policy == null)
                {
                    policy = Policy.NoOpAsync();//创建一个空的Policy
                    if (EnableCircuitBreaker)
                    {
                        // ExceptionsAllowedBeforeBreaking：Break the circuit after the specified number of consecutive exceptions，只在第一次运行出现这么多次，后面每出现一次就熔断
                        // MillisecondsOfBreak： and keep circuit broken for the specified duration.
                        policy = policy.WrapAsync(Policy.Handle<Exception>().CircuitBreakerAsync(ExceptionsAllowedBeforeBreaking, TimeSpan.FromMilliseconds(MillisecondsOfBreak)));
                    }
                    if (TimeOutMilliseconds > 0)
                    {
                        //超时执行Fallback逻辑
                        policy = policy.WrapAsync(Policy.TimeoutAsync(() => TimeSpan.FromMilliseconds(TimeOutMilliseconds), Polly.Timeout.TimeoutStrategy.Pessimistic));
                    }
                    if (MaxRetryTimes > 0)
                    {
                        //执行正常逻辑一次，然后执行重试：MaxRetryTimes次，最后执行Fallback
                        //比如MaxRetryTimes 配置1，则为：1+1+ 1(Fallback)=3
                        policy = policy.WrapAsync(Policy.Handle<Exception>().WaitAndRetryAsync(MaxRetryTimes, i => TimeSpan.FromMilliseconds(RetryIntervalMilliseconds)));
                    }

                    var policyFallBack = Policy
                    .Handle<Exception>()
                    .FallbackAsync(async (ctx, t) =>
                    {
                        AspectContext aspectContext = (AspectContext)ctx["aspectContext"];
                        //var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(this.FallBackMethod);
                        //merge this issue: https://github.com/yangzhongke/RuPeng.HystrixCore/issues/2
                        var fallBackMethod = context.ImplementationMethod?.DeclaringType?.GetMethod(this.FallBackMethod);
                        Object fallBackResult = fallBackMethod?.Invoke(context.Implementation, context.Parameters);

                        //不能如下这样，因为这是闭包相关，如果这样写第二次调用Invoke的时候context指向的
                        //还是第一次的对象，所以要通过Polly的上下文来传递AspectContext
                        //context.ReturnValue = fallBackResult;
                        aspectContext.ReturnValue = fallBackResult;
                    }, async (ex, t) => { });

                    policy = policyFallBack.WrapAsync(policy);
                    //放入
                    policies.TryAdd(context.ServiceMethod, policy);
                }
            }

            //var cache = context.ServiceProvider.Resolve<IDistributedCache>();

            //把本地调用的AspectContext传递给Polly，主要给FallbackAsync中使用，避免闭包的坑
            Context pollyCtx = new Context();
            pollyCtx["aspectContext"] = context;

            if (CacheTTLMinutes > 0 || CacheTTLSeconds > 0)
            {
                var isAsync = context.IsAsync();
                if (context.ServiceMethod.ReturnType == typeof(void) || isAsync && !context.ServiceMethod.ReturnType.IsGenericType)
                {
                    return;
                }

                var cacheTimeSpan = CacheTTLMinutes > 0 ? TimeSpan.FromMinutes(CacheTTLMinutes) : TimeSpan.FromSeconds(CacheTTLSeconds);

                //用类名+方法名+参数的下划线连接起来作为缓存key

                var accessor = context.ServiceProvider.Resolve<IHttpContextAccessor>();
                var userId = CacheGlobal ? string.Empty : accessor?.HttpContext?.UserId();
                string cacheKey = $"Hei:Hystrix:{(userId.IsNotNullOrEmpty() ? userId + ":" : string.Empty)}{context.ServiceMethod?.DeclaringType?.FullName}:{context?.ServiceMethod?.Name}:{CacheAspectUtils.GenerateKey(context.Parameters)}";

                if (CacheType == CacheTypeEnum.MemoryCache)
                {
                    var memoryCache = context.ServiceProvider.Resolve<IMemoryCache>();

                    //尝试去缓存中获取。如果找到了，则直接用缓存中的值做返回值
                    if (memoryCache.TryGetValue(cacheKey, out var cacheValue))
                    {
                        context.ReturnValue = cacheValue;
                    }
                    else
                    {
                        //如果缓存中没有，则执行实际被拦截的方法
                        await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                        //存入缓存中
                        using (var cacheEntry = memoryCache.CreateEntry(cacheKey))
                        {
                            cacheEntry.Value = context.ReturnValue;
                            cacheEntry.AbsoluteExpiration = DateTime.Now + cacheTimeSpan;
                        }
                    }
                }
                else
                {
                    var redis = context.ServiceProvider.Resolve<IDatabase>();
                    var serializerOptions = context.ServiceProvider.JsonSerializerOptions();

                    if (redis == null) throw new Exception("Redis Cache config error(The instance was not initialized)");

                    var cacheStr = await redis.StringGetAsync(cacheKey);
                    if (cacheStr.HasValue)
                    {
                        Type returnType = isAsync ? context.ServiceMethod.ReturnType.GetGenericArguments().First() : context.ServiceMethod.ReturnType;
                        var returnValue = JsonSerializer.Deserialize(cacheStr, returnType, serializerOptions);

                        if (isAsync)
                        {
                            var callFunc = context.ServiceMethod.ReturnType.GetTypeInfo().IsTaskWithResult() ? CacheAspectUtils.TaskResultFunc(returnType) : CacheAspectUtils.ValueTaskResultFunc(returnType);
                            context.ReturnValue = callFunc(returnValue!);
                        }
                        else
                        {
                            context.ReturnValue = returnValue;
                        }
                    }
                    else
                    {
                        await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                        object returnValue = context.IsAsync() ? await context.UnwrapAsyncReturnValue() : context.ReturnValue;
                        var valueStr = JsonSerializer.Serialize(returnValue, serializerOptions);
                        await redis.StringSetAsync(cacheKey, valueStr, cacheTimeSpan);
                    }
                }
            }
            else//如果没有启用缓存，就直接执行业务方法
            {
                await policy.ExecuteAsync(ctx => next(context), pollyCtx);
            }
        }
    }
}