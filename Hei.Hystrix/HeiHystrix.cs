using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hei.Hystrix
{
    public class HeiHystrix
    {
        public static async Task<T> Retry<T>(Func<Task<T>> taskFunc, int maxRetryTimes, int retryIntervalSeconds, Type onError = null)
        {
            var policy = PollyPolicy.Retry(maxRetryTimes, retryIntervalSeconds, onError);

            return await policy.ExecuteAsync(taskFunc);
        }

        public static async Task<T> CircuitBreaker<T>(Func<Task<T>> taskFunc, int exceptionsAllowedBeforeBreaking, int secondsOfBreak, Type onError = null)
        {
            var policy = PollyPolicy.CircuitBreaker(exceptionsAllowedBeforeBreaking, secondsOfBreak, onError);

            return await policy.ExecuteAsync(taskFunc);
        }

        public static async Task<T> Fallback<T>(Func<Task<T>> taskFunc, Action fallbackMethod, Type onError = null)
        {
            var policy = PollyPolicy.Fallback(fallbackMethod, onError);

            return await policy.Execute(taskFunc);
        }

        public static async Task<T> TimeOut<T>(Func<Task<T>> taskFunc, TimeSpan timeOut)
        {
            var policy = PollyPolicy.TimeOut(timeOut);

            return await policy.ExecuteAsync(taskFunc);
        }

        public static async Task<T> Bulkhead<T>(Func<Task<T>> taskFunc, int maxParallelization, int maxQueuingActions, Action onBulkheadRejected = null)
        {
            var policy = PollyPolicy.Bulkhead(maxParallelization, maxQueuingActions, onBulkheadRejected);

            return await policy.Execute(taskFunc);
        }
    }
}