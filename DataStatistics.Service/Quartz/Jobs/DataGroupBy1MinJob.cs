using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Quartz.Jobs.Interface;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Jobs
{
    public class DataGroupBy1MinJob : IDataGroupBy1MinJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DataGroupBy1MinJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;
        public DataGroupBy1MinJob(IEasyCachingProviderFactory providerFactory, ILogger<DataGroupBy1MinJob> logger)
        {
            _providerFactory = providerFactory;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                DateTime endtTime = DateTime.Now;
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*", 0);
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen(key);
                    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();

                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Execute:{e.Message}");
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
