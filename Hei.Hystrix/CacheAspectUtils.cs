using AspectCore.Extensions.Reflection;
using Hei.Infrastructure;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hei.Hystrix
{
    public class CacheAspectUtils
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _asTaskFuncCache = new ConcurrentDictionary<Type, Func<object, object>>();
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _asValueTaskFuncCache = new ConcurrentDictionary<Type, Func<object, object>>();

        public static Func<object, object> TaskResultFunc(Type returnType)
        {
            var func = _asTaskFuncCache.GetOrAdd(returnType, type =>
            {
                var resultMethod = typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(returnType);
                ParameterExpression source = Expression.Parameter(typeof(object), "result");
                var instanceCast = Expression.Convert(source, returnType);
                var callExpr = Expression.Call(resultMethod, instanceCast);
                var expr = Expression.Lambda<Func<object, object>>(callExpr, source).Compile();
                return expr;
            });
            return func;
        }

        public static Func<object, object> ValueTaskResultFunc(Type returnType)
        {
            var func = _asValueTaskFuncCache.GetOrAdd(returnType, type =>
            {
                var vauleType = typeof(ValueTask<>).MakeGenericType(returnType);
                ParameterExpression source = Expression.Parameter(typeof(object), "result");
                UnaryExpression instanceCast = Expression.Convert(source, returnType);
                var newExpr = Expression.New(vauleType.GetConstructor(new[] { returnType }), instanceCast);
                var convertBody = Expression.Convert(newExpr, typeof(object));
                var expr = Expression.Lambda<Func<object, object>>(convertBody, source).Compile();
                return expr;
            });
            return func;
        }

        public static string GenerateKey(params object[] paramters)
        {
            List<string> list = new List<string>();
            Type type = null;
            foreach (var item in paramters)
            {
                type = item.GetType();
                if (type != typeof(string) && item.GetType().IsEnumerable())
                {
                    var objs = (IEnumerable)item;
                    foreach (var obj in objs)
                    {
                        list.Add(obj.ToString());
                    }
                    continue;
                }
                if (type.BaseType is Object)
                {
                    list.Add(JsonSerializer.Serialize(item).JsonClearUp()?.Replace(":", "@"));
                }
                else
                {
                    list.Add(item?.ToString());
                }
            }
            return list.Any() ? string.Join('|', list.Where(i => !string.IsNullOrWhiteSpace(i))) : "";
        }
    }
}