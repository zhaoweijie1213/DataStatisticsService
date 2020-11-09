using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Api.Enums;
using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
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
        public DataStatisticsController(ILogger<DataStatisticsController> logger,IDataService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// 昨日概况
        /// </summary>
        /// <returns></returns>
        [HttpGet("DataSituationForYestoday")]
        public ApiResult<List<OverallSituationModel>> DataSituationForYestoday(int areaid)
        {
            try
            {
                var data = _service.DataSituationForYestoday(areaid);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<List<OverallSituationModel>>(ResultCode.Error, null, "操作失败");
            }
            
        }
        /// <summary>
        /// 30天数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpGet("ThirtyDaysData")]
        public ApiResult<ThirtyDaysDataModel> ThirtyDaysData(int areaid,DateTime time)
        {
            try
            {
                var data = _service.ThirtyDaysData(areaid, time);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<ThirtyDaysDataModel>(ResultCode.Error, null, "操作失败");
            }

        }
    }
}
