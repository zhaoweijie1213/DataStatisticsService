﻿using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Quartz.Jobs.Interface;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.Common;
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

        private readonly IMJLogOtherRepository _repository;
        public DataGroupBy5MinJob(ILogger<DataGroupBy5MinJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime endtTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:00"));
                //5分钟
                DateTime startTime = endtTime.AddMinutes(-5);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*", 0).Where(i => !i.Contains("r")).Where(i => {
                    return Regex.IsMatch(i, "^\\d+$");
                }).ToList();
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen(key);
                    var all_data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();
                    //获取版本号
                    List<string> vList = _repository.GetVersion(Convert.ToInt32(key));
                    //获取类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();

                    foreach (var type in dataType)
                    {
                        var tdata = all_data.Where(i => i.type == type).ToList();
                        foreach (var v in vList)
                        {
                            var data = tdata.Where(i => i.version == v).ToList();
                            List<JobRealData> reg = JobDataProcessing.GetDataList(data, endtTime);
                            //注册用户 5分钟时间粒度
                            redisProvider.RPush($"r_5_{type}_{v}_{key}", reg);
                            _logger.LogInformation($"5分钟时间粒度,版本{v},类别:{type}");
                        }
                        //所有版本
                        List<JobRealData> areg = JobDataProcessing.GetDataList(tdata, endtTime);
                        redisProvider.RPush($"r_5_{type}_{key}", areg);
                        _logger.LogInformation($"5分钟时间粒度,总数据,类别:{type}");
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
