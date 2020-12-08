using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Quartz.Jobs.Interface;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                //30天过期
                //DateTime time = DateTime.Now.AddDays(-30);
                //七天过期
                //DateTime time = DateTime.Now.AddDays(-7);
                DateTime time_1 = DateTime.Now.AddHours(-24);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("r_*", 0);
                //删除超过24小时的元素
                foreach (var key in keys)
                {
                    if (key.Contains("r"))
                    {
                        var length = redisProvider.LLen(key);
                        var data = redisProvider.LRange<JobRealData>(key, 0, length).Where(i => i.dateTime <= time_1).ToList();
                        foreach (var item in data)
                        {
                            redisProvider.LRem(key, 0, item);
                            _logger.LogInformation($"实时数据过期：{item}");
                        }
                    }
                    //else
                    //{
                    //    var length = redisProvider.LLen(key);
                    //    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date <= time).ToList();
                    //    foreach (var item in data)
                    //    {
                    //        redisProvider.LRem(key, 0, item);
                    //        _logger.LogInformation($"过期：{item}");
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Execute:{e.Message}");
            }
            return Task.CompletedTask;
        }
    }
}
