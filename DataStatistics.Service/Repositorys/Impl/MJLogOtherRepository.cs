using DataStatistics.Model.mj_log_other;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class MJLogOtherRepository : BaseRepository, IMJLogOtherRepository
    {
        private readonly ILogger<MJLogOtherRepository> _logger;
        public MJLogOtherRepository(ILogger<MJLogOtherRepository> logger, string ConnectionString)
        {
            _logger = logger;
            base.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 获取用户活动元数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<UserActionModel> GetUserActions()
        {
            try
            {
                var res = _db.Sql("select * from log_userAction where date <='2020-11-05' and date>'2020-11-04' ").QueryMany<UserActionModel>();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserActions:{e.Message}");
                throw;
            }
        }
    }
}
