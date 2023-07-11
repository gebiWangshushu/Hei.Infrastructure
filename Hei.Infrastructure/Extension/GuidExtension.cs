using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hei.Infrastructure
{
    public static class GuidExtension
    {
        public static long ToInt64(this Guid guid) => BitConverter.ToInt64(guid.ToByteArray(), 0);
    }
}