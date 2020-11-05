using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Api.Enums;
using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Services;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataStatistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataStatisticsController : BaseController
    {
        private readonly ILogger<DataStatisticsController> _logger;
        private readonly IDataService _service;
        private readonly ICacheManage  _cache;
        public DataStatisticsController(ILogger<DataStatisticsController> logger,IDataService service, ICacheManage cache)
        {
            _logger = logger;
            _service = service;
            _cache = cache;
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetResult")]
        public ApiResult<List<UserActionModel>> GetResult()
        {
            try
            {
                var data = _service.GetUserActions();
                //存入缓存
                _cache.SetUserAction(data);
                var res = _cache.GetUserAction();
                return Json(ResultCode.Success, res.Take(100).ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<List<UserActionModel>>(ResultCode.Error, null, "操作失败");
            }
            
        }
    }
}
