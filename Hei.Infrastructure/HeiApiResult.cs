using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Hei.Infrastructure
{
    public class HeiApiResult<TData>
    {
        /// <summary>
        /// 响应代码
        /// </summary>
        //[JsonPropertyName("code")]
        [Required]
        public EnumStatus Code { get; set; }

        /// <summary>
        /// 响应信息，调用失败时一定有响应信息
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 响应数据，可能为空
        /// </summary>
        public TData Data { get; set; }

        [JsonIgnore]
        public bool Success => this.Code == EnumStatus.Success;
    }

    public enum EnumStatus
    {
        /// <summary>
        /// 调用成功
        /// </summary>
        [Description("调用成功")]
        Success = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("调用失败")]
        Fail = 0,

        /// <summary>
        /// 未授权/sessionKey不存在或已过期
        /// </summary>
        [Description("未授权/sessionKey不存在或已过期")]
        UnAuthorized = 401,

        /// <summary>
        /// token授权通过，但没有本接口权限
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 参数欠缺
        /// </summary>
        [Description("参数欠缺")]
        ParamsLost = 411,

        /// <summary>
        /// 参数格式有误
        /// </summary>
        [Description("参数格式有误")]
        ParamsFormatWrong = 412,

        /// <summary>
        /// 参数有误,超范围等
        /// </summary>
        [Description("参数有误")]
        ParamsWrong = 413,
    }
}