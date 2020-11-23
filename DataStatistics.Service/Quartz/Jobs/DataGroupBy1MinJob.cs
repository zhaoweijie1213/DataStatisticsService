﻿using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
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
                DateTime endtTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:00"));
                DateTime startTime = endtTime.AddMinutes(-1);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<string> keys = redisProvider.SearchKeys("*", 0).Where(i => !i.Contains("r")).ToList(); 
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen(key);
                    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();
                   
                    List<JobRealData> reg = new List<JobRealData>() {
                        new JobRealData()
                        {
                            //活跃用户
                            Active=new plat()
                            {
                                 All=data.Where(i=>i.uid!=0).GroupBy(i=>i.uid).Count(),
                                 Android=data.Where(i=>i.uid!=0&&i.platForm==PlatFromEnum.Android.GetName()).GroupBy(i=>i.uid).Count(),
                                 IOS=data.Where(i=>i.uid!=0&&i.platForm==PlatFromEnum.IOS.GetName()).GroupBy(i=>i.uid).Count(),
                                 Windows=data.Where(i=>i.uid!=0&&i.platForm==PlatFromEnum.Windows.GetName()).GroupBy(i=>i.uid).Count()
                            },
                             //注册用户
                            Register=new plat()
                            {
                                All=data.Where(i=>i.uid==0).Count(),
                                Android=data.Where(i=>i.uid==0&&i.platForm==PlatFromEnum.Android.GetName()).Count(),
                                IOS=data.Where(i=>i.uid==0&&i.platForm==PlatFromEnum.Android.GetName()).Count(),
                                Windows=data.Where(i=>i.uid==0&&i.platForm==PlatFromEnum.Android.GetName()).Count(),
                            },
                            dateTime=endtTime
                        }
                    };
                  
                    //1分钟时间粒度
                    redisProvider.RPush($"r_1_{key}", reg);
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
