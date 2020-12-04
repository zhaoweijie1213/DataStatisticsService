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
    public class DataGroupBy10MinJob : IDataGroupBy10MinJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DataGroupBy10MinJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;

        private readonly IMJLogOtherRepository _repository;
        public DataGroupBy10MinJob(ILogger<DataGroupBy10MinJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository)
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
                DateTime startTime = endtTime.AddMinutes(-10);
                string ketTime = DateTime.Now.ToString("yyyyMMddHH");
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*", 0).Where(i => !i.Contains("r")).Where(i => {
                    return Regex.IsMatch(i, "^\\d+$");
                }).ToList();
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen($"{key}_t{ketTime}");
                    //获取一个小时内所有数据
                    var all_data = redisProvider.LRange<UserActionModel>($"{key}_t{ketTime}", 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();

                    //获取版本号
                    List<string> vList = _repository.GetAreaVersion(Convert.ToInt32(key)).Select(i => i.version).ToList();
                    //获取类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();
                    foreach (var type in dataType)
                    {
                        var tdata = all_data.Where(i => i.type == type).ToList();
                        foreach (var v in vList)
                        {
                            var data = tdata.Where(i => i.version == v).ToList();
                            List<JobRealData> reg = JobDataProcessing.GetDataList(data, endtTime);
                            // 10分钟时间粒度
                            redisProvider.RPush($"r_10_{type}_{v}_{key}", reg);
                            //_logger.LogInformation($"{key}大厅,10分钟时间粒度,版本{v},类别:{type}");
                        }
                        List<JobRealData> areg = JobDataProcessing.GetDataList(tdata, endtTime);
                        // 10分钟时间粒度
                        redisProvider.RPush($"r_10_{type}_{key}", areg);
                        redisProvider.KeyExpire($"r_10_{type}_{key}", (int)KeyExpireTime.realData);
                        _logger.LogInformation($"{key}大厅,10分钟时间粒度,类别:{type}");
                    }
                 
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
