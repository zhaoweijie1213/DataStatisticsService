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
        public ApiResult<DaysDataModel> ThirtyDaysData(int areaid,DateTime time)
        {
            try
            {
                var data = _service.ThirtyDaysData(areaid, time);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<DaysDataModel>(ResultCode.Error, null, "操作失败");
            }

        }
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type">0:秒,1:分,2:时</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("RealTimeData")]
        public ApiResult<DaysDataModel> RealTimeData(int areaid,int type,int value)
        {

            try
            {
                var data = _service.RealTimeData(areaid, type,value);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<DaysDataModel>(ResultCode.Error, null, "操作失败");
            }
        }
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        [HttpGet("GetAreaParams")]
        public ApiResult<AreaParamsModel> GetAreaParams(int areaid) 
        {

            try
            {
                var data = _service.GetAreaParams(areaid);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<AreaParamsModel>(ResultCode.Error, null, "操作失败");
            }
        }
    }
}
