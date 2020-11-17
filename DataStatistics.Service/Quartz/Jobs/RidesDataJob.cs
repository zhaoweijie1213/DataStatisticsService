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
    public class RidesDataJob : IRidesDataJob
    {
        private readonly ILogger<RidesDataJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;
        public RidesDataJob(IEasyCachingProviderFactory providerFactory, ILogger<RidesDataJob> logger)
        {
            _providerFactory = providerFactory;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime time = DateTime.Now.AddHours(-24);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*", 0);
                //删除超过24小时的元素
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen(key);
                    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date <= time).ToList();
                    foreach (var item in data)
                    {
                        redisProvider.LRem(key, 0, item);
                    }
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
