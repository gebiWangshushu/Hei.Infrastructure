using Polly.Retry;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using Polly.Fallback;
using AspectCore.DynamicProxy;
using Polly.Timeout;
using System.ComponentModel.DataAnnotations;
using Polly.Bulkhead;

namespace Hei.Hystrix
{
    public class PollyPolicy
    {
        public static AsyncRetryPolicy Retry(int maxRetryTimes, int retryIntervalSeconds, Type onError = null)
        {
            if (onError == null)
            {
                onError = typeof(Exception);
            }

            return Policy
                .Handle<Exception>(ex =>
                {
                    if (onError == typeof(Exception))
                    {
                        return true;
                    }
                    else
                    {
                        return (ex.GetType() == onError || ex.GetType().IsSubclassOf(onError));
                    }
                })
                .WaitAndRetryAsync(maxRetryTimes, _ => TimeSpan.FromSeconds(retryIntervalSeconds));
        }

        public static AsyncCircuitBreakerPolicy CircuitBreaker(int exceptionsAllowedBeforeBreaking, int secondsOfBreak, Type onError = null)
        {
            if (onError == null)
            {
                onError = typeof(Exception);
            }

            return Policy.Handle<Exception>(ex =>
             {
                 if (onError == typeof(Exception))
                 {
                     return true;
                 }
                 else
                 {
                     return (ex.GetType() == onError || ex.GetType().IsSubclassOf(onError));
                 }
             })
             .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking, TimeSpan.FromSeconds(secondsOfBreak));
        }

        public static FallbackPolicy Fallback(Action fallbackMethod, Type onError = null)
        {
            if (onError == null)
            {
                onError = typeof(Exception);
            }

            return Policy.Handle<Exception>(ex =>
            {
                if (onError == typeof(Exception))
                {
                    return true;
                }
                else
                {
                    return (ex.GetType() == onError || ex.GetType().IsSubclassOf(onError));
                }
            })
            .Fallback(fallbackMethod);
        }

        public static AsyncTimeoutPolicy TimeOut(TimeSpan timeout)
        {
            return Policy.TimeoutAsync(() => timeout, Polly.Timeout.TimeoutStrategy.Pessimistic);
        }

        public static BulkheadPolicy Bulkhead(int maxParallelization, int maxQueuingActions, Action onBulkheadRejected = null)
        {
            return Policy
               .Bulkhead(maxParallelization, maxQueuingActions, onBulkheadRejected: context =>
               {
                   if (onBulkheadRejected != null)
                   {
                       onBulkheadRejected();
                   }
               });
        }
    }
}