using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ApiResult<List<OverallSituationModel>> DataSituationForYestoday(int areaid, int type)
        {
            try
            {
                var data = _service.DataSituationForYestoday(areaid,type);
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
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpGet("ThirtyDaysData")]
        public ApiResult<DaysDataModel> ThirtyDaysData(int areaid, int type, DateTime time)
        {
            try
            {
                var data = _service.ThirtyDaysData(areaid, type, time);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<DaysDataModel>(ResultCode.Error, null, "操作失败");
            }

        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        [HttpGet("GetVersion")]
        public ApiResult<List<string>> GetVersion(int areaid)
        {
            try
            {
                var data = _service.GetVersion(areaid);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<List<string>>(ResultCode.Error, null, "操作失败");
            }
        }
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet("RealTimeData")]
        public ApiResult<DaysDataModel> RealTimeData(int areaid, int value, int type, string version)
        {

            try
            {
                //Stopwatch MyStopWatch = new Stopwatch();
                //MyStopWatch.Start();
                var data = _service.RealTimeData(areaid, value, type, version);
                //MyStopWatch.Stop();
                //_logger.LogInformation($"GetResult耗时:{MyStopWatch.ElapsedTicks / 10000000}s,");
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
        /// <param name="areaid">大厅</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [HttpGet("GetAreaParams")]
        public ApiResult<AreaParamsModel> GetAreaParams(int areaid,int type) 
        {
            try
            {
                var data = _service.GetAreaParams(areaid,type);
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
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet("GetUserPic")]
        public ApiResult<UserPicModel> GetUserPic(int areaid,DateTime start,DateTime end,int type,string version)
        {
            try
            {

                var data = _service.GetUserPic(areaid, start, end, type, version);
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
        /// <param name="type">类型</param>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        [HttpGet("GetSingleSceneData")]
        public ApiResult<SingleSceneModel> GetSingleSceneData(int areaid, int days, string platFrom, string otherParam, string dateRange, int type, string version)
        {
            try
            {
                var data = _service.GetSingleSceneData(areaid, days, platFrom, otherParam, dateRange,type,version);
                return Json(ResultCode.Success, data);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResult:{e.Message}");
                return Json<SingleSceneModel>(ResultCode.Error, null, "操作失败");
            }
        }
        ///// <summary>
        ///// 漏斗图
        ///// </summary>
        ///// <param name="areaid"></param>
        ///// <param name="platForm"></param>
        ///// <param name="days"></param>
        ///// <param name="start"></param>
        ///// <param name="end"></param>
        ///// <param name="other"></param>
        ///// <param name="otherValue"></param>
        ///// <param name="type"></param>
        ///// <param name="version"></param>
        ///// <returns></returns>
        //[HttpGet("GetFunnelData")]
        //public ApiResult<FunnelDataModel> GetFunnelData(int areaid, string platForm, int days, DateTime? start, DateTime? end, string other, string otherValue, int type, string version)
        //{
        //    try
        //    {
        //        var data = _service.GetFunnelData(areaid, platForm, days, start, end, other, otherValue, type, version);
        //        return Json(ResultCode.Success, data);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"GetResult:{e.Message}");
        //        return Json<FunnelDataModel>(ResultCode.Error, null, "操作失败");
        //    }
        //}
    }
}
