using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hei.Infrastructure
{
    /// <summary>
    /// 定义controllers的基础行为
    /// </summary>
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("v1/[controller]/[action]")]
    [ProducesResponseType(typeof(HeiApiResult<object>), 200)]
    [ApiExplorerSettings(GroupName = "v1")]
    public class HeiApiController : ControllerBase
    {
        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        protected IActionResult Success()
        {
            return Ok(new HeiApiResult<object> { Code = EnumStatus.Success });
        }

        protected IActionResult Success(string msg)
        {
            return Ok(new HeiApiResult<object> { Code = EnumStatus.Success, Message = msg });
        }

        protected IActionResult Success<TData>(string Msg, TData data)
        {
            return Ok(new HeiApiResult<TData> { Code = EnumStatus.Success, Message = Msg, Data = data });
        }

        protected IActionResult Fail()
        {
            return Ok(new HeiApiResult<object> { Code = EnumStatus.Fail });
        }

        protected IActionResult Fail(string msg)
        {
            return Ok(new HeiApiResult<object> { Code = EnumStatus.Fail, Message = msg });
        }

        protected IActionResult Fail<TData>(string Msg, TData data)
        {
            return Ok(new HeiApiResult<TData> { Code = EnumStatus.Fail, Message = Msg, Data = data });
        }

        protected IActionResult SiluzanApiResult(bool success, string msg)
        {
            return Ok(new HeiApiResult<object> { Code = success ? EnumStatus.Success : EnumStatus.Fail, Message = msg });
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult SiluzanApiResult<TData>((bool success, string Msg, TData Data) result)
        {
            return Ok(new HeiApiResult<TData> { Code = result.success ? EnumStatus.Success : EnumStatus.Fail, Message = result.Msg, Data = result.Data });
        }
    }
}