using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hei.Infrastructure
{
    public static class AppSettings
    {
        public static IConfiguration Configuration;
        private static bool _isDevelopment;
        private static bool _isStaging;
        private static bool _isCI;
        private static bool _isProduction;

        public static string EnvironmentName;

        public static void InitAppSettings(this IConfiguration config, string environmentName)
        {
            if (Configuration == null)
            {
                Configuration = config;
            }

            EnvironmentName = environmentName;

            _isDevelopment = environmentName.Equals("Development");
            _isStaging = environmentName.Equals("Staging");
            _isCI = environmentName.Equals("CI");
            _isProduction = environmentName.Equals("Production");
        }

        public static bool IsDevelopment() => _isDevelopment;

        public static bool IsStaging() => _isStaging;

        public static bool IsCI() => _isCI;

        public static bool IsProduction() => _isProduction;

        /// <summary>
        /// 获取单个简单配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="abortWhenNull">为空是否报错</param>
        /// <returns></returns>
        public static string Get(string key, bool abortWhenNull = true)
        {
            var value = getConfiguration()[key];
            //Console.WriteLine($"appsetting:key:{key} value:{value}");
            if (string.IsNullOrEmpty(value) && abortWhenNull)
            {
                throw new ArgumentNullException(key, $"config is none or empty!");
            }
            return value;
        }

        /// <summary>
        /// 获取单个实体配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="abortWhenNull"></param>
        /// <returns></returns>
        public static T Get<T>(string key, bool abortWhenNull = true) where T : class
        {
            var section = GetSection(key);
            var value = section != null ? section.Get<T>() : default(T);
            if (value == null && abortWhenNull)
            {
                throw new ArgumentNullException(key, $"config is none or empty!");
            }
            return value;
        }

        public static bool GetBool(string key)
        {
            bool isTrue = false;
            if (!string.IsNullOrEmpty(key) && Configuration != null)
            {
                bool.TryParse(Get(key), out isTrue);
            }
            return isTrue;
        }

        /// <summary>
        /// 获取int配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int GetInt(string key, int defaultValue = int.MinValue)
        {
            int intValue = 0;
            if (!string.IsNullOrEmpty(key) && Configuration != null)
            {
                var parseResult = int.TryParse(Get(key, false), out intValue);
                if (parseResult == false && defaultValue != int.MinValue)
                {
                    return defaultValue;
                }
            }

            return intValue;
        }

        /// <summary>
        /// 获取某个section中指定key的配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valIndex"></param>
        /// <returns></returns>
        public static string GetSection(string key, string valIndex)
        {
            return getConfiguration().GetSection(key)[valIndex];
        }

        /// <summary>
        ///  获取指定section
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSection(string key)
        {
            return getConfiguration().GetSection(key);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            var connstr = getConfiguration().GetConnectionString(name);
            if (connstr.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(name, $"connectionString {name} is none or empty!");
            }
            return connstr;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static bool IsFunctionApp() => "FUNCTION_APP" == Environment.GetEnvironmentVariable("NETCORE_APP_TYPE");

        private static IConfiguration getConfiguration()
        {
            if (Configuration == null)
            {
                throw new NullReferenceException("Configuration is null, please call InitAppSettings() before use.");
            }
            return Configuration;
        }
    }
}