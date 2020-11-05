using DataStatistics.Model.mj_log_other;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class MJLogOtherRepository : BaseRepository, IMJLogOtherRepository
    {
        private ILogger<MJLogOtherRepository> _logger;
        public MJLogOtherRepository(ILogger<MJLogOtherRepository> logger, string ConnectionString) :base(ConnectionString)
        {
            _logger = logger;
        }
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetUserActions()
        {
            try
            {
                var res = _db.Sql("select *from log_userAction").QueryMany<UserActionModel>();
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
