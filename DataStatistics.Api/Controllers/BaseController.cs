using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Api.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataStatistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ApiResult<T> Json<T>(ResultCode code, T data, string msg="操作成功")
        {
            var result = new ApiResult<T>()
            {
                Code = (int)code,
                Message = msg,
                Result = data
            };
            return result;
        }
    }
}
