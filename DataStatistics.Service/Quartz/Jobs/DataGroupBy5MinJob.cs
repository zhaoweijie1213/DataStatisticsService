using DataStatistics.Service.Quartz.Jobs.Interface;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Jobs
{
    public class DataGroupBy5MinJob : IDataGroupBy5MinJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DataGroupBy5MinJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;
        public DataGroupBy5MinJob(ILogger<DataGroupBy5MinJob> logger, IEasyCachingProviderFactory providerFactory)
        {
            _logger = logger;
            _providerFactory = providerFactory;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {

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
