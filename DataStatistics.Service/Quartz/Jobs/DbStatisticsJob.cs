﻿using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Impl
{
    /// <summary>
    /// 数据处理任务
    /// </summary>
    public class DbStatisticsJob : IJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DbStatisticsJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMJLogOtherRepository _repository;
        public DbStatisticsJob(ILogger<DbStatisticsJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime startTime = DateTime.Now.Date.AddDays(-1);
                DateTime endtTime = DateTime.Now.Date;
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*",0);
                foreach (var key in keys)
                {
                    var data = redisProvider.SMembers<UserActionModel>(key);
                    List<OverallSituationModel> list = new List<OverallSituationModel>();
                    //all
                    OverallSituationModel all = new OverallSituationModel() {
                        areaid = Convert.ToInt32(key),
                        activeUsers = data.Where(i => i.uid != 0).Count(),
                        registeredUsers = data.Where(i => i.uid == 0).Count(),
                        platForm = "All",
                        dataTime= startTime,
                    };
                    list.Add(all);
                    //windows
                    OverallSituationModel windows = new OverallSituationModel()
                    {
                        areaid = Convert.ToInt32(key),
                        activeUsers = data.Where(i => i.uid != 0 && i.platForm == "Windows").Count(),
                        registeredUsers = data.Where(i => i.uid == 0 && i.platForm == "Windows").Count(),
                        platForm = "Windows",
                        dataTime = startTime,
                    };
                    list.Add(windows);
                    //ios
                    OverallSituationModel ios = new OverallSituationModel()
                    {
                        areaid = Convert.ToInt32(key),
                        activeUsers = data.Where(i => i.uid != 0 && i.platForm == "IOS").Count(),
                        registeredUsers = data.Where(i => i.uid == 0 && i.platForm == "IOS").Count(),
                        platForm = "IOS",
                        dataTime = startTime,
                    };
                    list.Add(ios);
                    //android
                    OverallSituationModel android = new OverallSituationModel()
                    {
                        areaid = Convert.ToInt32(key),
                        activeUsers = data.Where(i => i.uid != 0 && i.platForm == "Android").Count(),
                        registeredUsers = data.Where(i => i.uid == 0 && i.platForm == "Android").Count(),
                        platForm = "Android",
                        dataTime = startTime,
                    };
                    list.Add(android);
                    var res = _repository.Insert(list);
                    _logger.LogInformation($"更新:{res}条数据,时间:{DateTime.Now:yyyy-MMM-dd HH:mm:ss:ffff}");
                    //删除redis缓存
                    redisProvider.SRem<UserActionModel>(key);
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
