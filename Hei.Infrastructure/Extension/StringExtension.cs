using System;
using System.Text.RegularExpressions;

namespace Hei.Infrastructure
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool IsGuid(this string str)
        {
            Guid x;
            return Guid.TryParse(str, out x);
        }

        public static Guid? ToGuid(this string str) => str == null ? null : (Guid?)Guid.Parse(str);

        /// <summary>
        /// 匹配剪辑timspan格式：00:00:01.300
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsClipTimeSpan(this string str)
        {
            return Regex.IsMatch(str, @"^\d{2}\:\d{2}\:\d{2}\.\d{3}$");
        }

        /// <summary>
        /// 删掉xml中某些多余字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string XmlCleanup(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            return str.Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// 获取url中的文件名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetUrlName(this string str)
        {
            Uri uri = null;
            Uri.TryCreate(str, UriKind.Absolute, out uri);
            if (uri != null)
            {
                return uri.LocalPath.Replace("/", "");
            }
            return str;
        }

        /// <summary>
        /// guid字符串转long类型.
        /// </summary>
        /// <param name="guidStr">The unique identifier string.</param>
        /// <returns></returns>
        public static long ToInt64(this string guidStr) => new Guid(guidStr).ToInt64();

        /// <summary>
        /// 去掉json的那些符号
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonClearUp(this string json)
        {
            if (json.IsNotNullOrEmpty())
            {
                return json.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\\", "").Replace(" ", "").Replace(@"""", "");
            }
            return string.Empty;
        }
    }
}