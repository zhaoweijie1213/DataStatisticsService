using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Impl
{
    public class DbStatisticsJob : IJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private ILogger<DbStatisticsJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private IEasyCachingProviderFactory _providerFactory;
        /// <summary>
        /// 
        /// </summary>
        private IMJLogOtherRepository _repository;
        public DbStatisticsJob(ILogger<DbStatisticsJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            //IServiceProvider sp = (IServiceProvider)context.MergedJobDataMap.Get("Provider");
            //logger = (ILogger<DbStatisticsJob>)sp.GetService(typeof(ILogger<DbStatisticsJob>));
            //repository = (IMJLogOtherRepository)context.MergedJobDataMap.Get("repository");
            //providerFactory = (IEasyCachingProviderFactory)sp.GetService(typeof(IEasyCachingProviderFactory));
            try
            {
                DateTime startTime = DateTime.Now.Date;
                DateTime endtTime = DateTime.Now.Date.AddDays(-1);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*",0);
                foreach (var key in keys)
                {
                    var data = redisProvider.SMembers<UserActionModel>(key);
                    var datas = _repository.GetUserActions();
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
