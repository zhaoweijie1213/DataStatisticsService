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
        /// <summary>
        /// 用户画像
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet("GetUserPic")]
        public ApiResult<UserPicModel> GetUserPic(int areaid,DateTime start,DateTime end)
        {
            try
            {
                var data = _service.GetUserPic(areaid, start, end);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<UserPicModel>(ResultCode.Error, null, "操作失败");
            }
        }
        /// <summary>
        /// 单场景分析
        /// </summary>
        /// <param name="areaid">区域id</param>
        /// <param name="days">天数</param>
        /// <param name="platFrom">平台</param>
        /// <param name="otherParam">其它参数</param>
        /// <param name="dateRange">日期范围</param>
        /// <returns></returns>
        [HttpGet("GetSingleSceneData")]
        public ApiResult<SingleSceneModel> GetSingleSceneData(int areaid, int days, string platFrom, string otherParam, string dateRange)
        {
            try
            {
                var data = _service.GetSingleSceneData(areaid, days, platFrom, otherParam, dateRange);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<SingleSceneModel>(ResultCode.Error, null, "操作失败");
            }
        }
        /// <summary>
        /// 漏斗图
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="platForm"></param>
        /// <param name="days"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="other"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        [HttpGet("GetFunnelData")]
        public ApiResult<FunnelDataModel> GetFunnelData(int areaid, string platForm, int days, DateTime? start, DateTime? end, string other, string otherValue="")
        {
            try
            {
                var data = _service.GetFunnelData(areaid, platForm, days, start, end, other, otherValue);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<FunnelDataModel>(ResultCode.Error, null, "操作失败");
            }
        }
    }
}
