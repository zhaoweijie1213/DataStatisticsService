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
        private readonly IMJLog3Repository _mjlog3repository;
        public DataGroupBy5MinJob(ILogger<DataGroupBy5MinJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var time = DateTime.Now.AddMinutes(-5);
                DateTime endtTime = Convert.ToDateTime(time.ToString("yyyy-MM-dd HH:mm:00"));
                //5分钟
                DateTime startTime = endtTime.AddMinutes(-5);
                string ketTime = "";
                if (time.Minute > 0)
                {
                    ketTime = time.AddHours(1).ToString("yyyyMMddHH");
                }
                else
                {
                    ketTime = time.ToString("yyyyMMddHH");
                }
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<int> gameids = _mjlog3repository.GetGameid();
                foreach (var item in gameids)
                {
                    int key = item * 100;
                    var length = redisProvider.LLen($"{key}_t{ketTime}");
                    //获取一个小时内所有数据
                    var all_data = redisProvider.LRange<UserActionModel>($"{key}_t{ketTime}", 0, length).Where(i => i.date > startTime && i.date <= endtTime).ToList();
                    //获取版本号
                    List<string> vList = _repository.GetAreaVersion(Convert.ToInt32(key)).Select(i => i.version).ToList();
                    //获取类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();

                    foreach (var type in dataType)
                    {
                        var tdata = all_data.Where(i => i.type == type).ToList();
                        //所有版本
                        List<JobRealData> areg = JobDataProcessing.GetDataList(tdata, "", endtTime);
                        redisProvider.RPush($"r_5_{type}_{key}", areg);
                        //redisProvider.KeyExpire($"r_5_{type}_{key}", (int)KeyExpireTime.realData);
                        foreach (var v in vList)
                        {
                            var data = tdata.Where(i => i.version == v).ToList();
                            List<JobRealData> reg = JobDataProcessing.GetDataList(data,v, endtTime);
                            //注册用户 5分钟时间粒度
                            redisProvider.RPush($"r_5_{type}_{v}_{key}", reg);
                            //redisProvider.KeyExpire($"r_5_{type}_{v}_{key}", (int)KeyExpireTime.realData);
                            //_logger.LogInformation($"{key}大厅,5分钟时间粒度,版本{v},类别:{type}");
                        }
                        _logger.LogInformation($"{key}大厅,5分钟时间粒度,类别:{type}");
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
