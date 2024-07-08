using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hei.Hystrix
{
    //public static class TaskExtension
    //{
    //    /// <summary>
    //    /// v1 (存在进入不了task调试的问题，可能因为 task 是一个已经开始执行的任务，而 Polly 的 ExecuteAsync 期望的是一个返回新的任务的委托)
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="task"></param>
    //    /// <param name="retryCount"></param>
    //    /// <param name="retryIntervalMilliseconds"></param>
    //    /// <param name="exceptionType"></param>
    //    /// <returns></returns>
    //    public static async Task<T> RetryV1<T>(this Task<T> task, int retryCount, int retryIntervalMilliseconds, Type exceptionType = null)
    //    {
    //        if (exceptionType == null)
    //        {
    //            exceptionType = typeof(Exception);
    //        }

    //        var retryPolicy = Policy
    //            .Handle<Exception>(ex => ex.GetType() == exceptionType || ex.GetType().IsSubclassOf(exceptionType))
    //            .WaitAndRetryAsync(retryCount, _ => TimeSpan.FromMilliseconds(retryIntervalMilliseconds))
    //            //.RetryAsync(retryCount)
    //            ;

    //        //return await retryPolicy.ExecuteAsync(() => task);

    //        T result = default(T);
    //        await retryPolicy.ExecuteAsync(async () =>
    //        {
    //            result = await task;
    //        });

    //        return result;
    //    }

    //    public static async Task<T> RetryV2<T>(this Func<Task<T>> taskFunc, int retryCount, int retryIntervalMilliseconds, Type exceptionType = null)
    //    {
    //        if (exceptionType == null)
    //        {
    //            exceptionType = typeof(Exception);
    //        }

    //        var retryPolicy = Policy
    //            .Handle<Exception>(ex =>
    //            {
    //                var match = ex.GetType() == exceptionType;
    //                return match;
    //            })
    //            .WaitAndRetryAsync(retryCount, _ => TimeSpan.FromMilliseconds(retryIntervalMilliseconds));

    //        return await retryPolicy.ExecuteAsync(taskFunc);
    //    }
    //}

    //public static class FuncExtensions
    //{
    //    public static Func<Task<T>> ToFunc<T>(this Func<Task<T>> func)
    //    {
    //        return func;
    //    }
    //}
}