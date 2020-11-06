using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.Impl
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IMJLogOtherRepository _repository;
        private readonly ICacheManage _cache;
        public DataService(IMJLogOtherRepository repository, ILogger<DataService> logger, ICacheManage cache)
        {
            _logger = logger;
            _repository = repository;
            _cache = cache;
        }
        /// <summary>
        /// 数据库获取数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetUserActions()
        {
            try
            {
                var res = _repository.GetUserActions();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserActions:{e.Message}");
                throw;
            }
        }

        ///// <summary>
        ///// 昨日概况
        ///// </summary>
        ///// <returns></returns>
        public List<OverallSituationModel> DataSituationForYestoday(int areaid)
        {
            try
            {
                var res = _repository.GetSituation(areaid);
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"DataSituationForYestoday:{e.Message}");
                throw;
            } 
        }

        /// <summary>
        /// 近期趋势
        /// </summary>
        /// <returns></returns>
        public string TrendsForAFewDays()
        {
            return "";
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <returns></returns>
        public string ActiveData()
        {
            return "";
        }


    }
}
