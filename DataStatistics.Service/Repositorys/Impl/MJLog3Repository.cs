using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class MJLog3Repository : BaseRepository, IMJLog3Repository
    {
        private readonly ILogger<MJLog3Repository> _logger;
        public MJLog3Repository(ILogger<MJLog3Repository> logger, string ConnectionString)
        {
            _logger = logger;
            base.ConnectionString = ConnectionString;
        }
        /// <summary>
        /// 获取大厅id
        /// </summary>
        /// <returns></returns>
        public List<int> GetGameid()
        {
            string sql = "";
            try
            {
                sql = $"select gameid from config_game where parentCodeNo=0";
                var res = _db.Query<int>(sql).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetGameid:{e.Message}");
                return null;
            }
        }
    }
}
