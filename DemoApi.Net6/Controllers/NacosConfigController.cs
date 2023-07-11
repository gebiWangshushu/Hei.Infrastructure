using Microsoft.AspNetCore.Mvc;
using Hei.Infrastructure;

namespace DemoApi.Net6.Controllers
{
    [Route("nacos/config/[action]")]
    public class NacosConfigController : HeiApiController
    {
        private readonly IConfiguration _configuration;

        public NacosConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// ʹ��ϵͳ����IConfiguration��ȡ����ʾ��
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string key)
        {
            var config = _configuration[key];
            return Success("success", config);
        }

        /// <summary>
        /// ��ȡ�ַ���ʾ��
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSiluzanConfig(string key = "key1")
        {
            var config = AppSettings.Get(key);
            return Success("success", config);
        }

        /// <summary>
        ///  ��ȡ����ʾ��
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSiluzanConfigObj(string key = "key2")
        {
            var config = AppSettings.Get<Dictionary<string, string>>(key);
            return Success("success", config);
        }
    }
}