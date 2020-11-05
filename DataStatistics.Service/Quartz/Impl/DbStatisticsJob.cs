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
        /// 数据库仓库
        /// </summary>
        private IMJLogOtherRepository _repository;

        public Task Execute(IJobExecutionContext context)
        {
            IServiceProvider sp = (IServiceProvider)context.MergedJobDataMap.Get("Provider");
            _logger = (ILogger<DbStatisticsJob>)sp.GetService(typeof(ILogger<DbStatisticsJob>));
            _repository = (IMJLogOtherRepository)sp.GetService(typeof(IMJLogOtherRepository));
            try
            {
                //var data=
            }
            catch (Exception e)
            {
                _logger.LogError($"Execute:{e.Message}");
            }
            return Task.CompletedTask;
        }
    }
}
