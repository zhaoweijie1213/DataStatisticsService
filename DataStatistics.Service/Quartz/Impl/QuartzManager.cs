using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Quartz.Jobs.Interface;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Quartz.Impl
{
    public class QuartzManager : IQuartzManager
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<QuartzManager> _logger;
        /// <summary>
        /// 缓存
        /// </summary>
        readonly IEasyCachingProviderFactory _factory;
        readonly ICacheManage _cache;

        private readonly IMJLogOtherRepository _repository;
        private readonly IMJLog3Repository _mjlog3repository;
        public IScheduler Scheduler { get; set; }

        public QuartzManager(ILogger<QuartzManager> logger, IServiceProvider IocContainer, IEasyCachingProviderFactory factory, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository, ICacheManage cache)
        {
            _logger = logger;
            var schedulerFactory = new StdSchedulerFactory();
            IOCJobFactory iocJobfactory = new IOCJobFactory(IocContainer);
            Scheduler = schedulerFactory.GetScheduler().Result;
            Scheduler.JobFactory = iocJobfactory;
            Scheduler.Start().Wait();
            _factory = factory;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
            _cache = cache;
        }
        /// <summary>
        /// 初始化加载调度任务
        /// </summary>
        public void LoadScheduleJob(IServiceProvider serviceProvider)
        {
            try
            {
                DateTimeOffset time = DateTimeOffset.Now.AddMinutes(5);
                #region 昨日概况
                //昨日概况
                var job = JobBuilder.Create<IDbStatisticsJob>()
                    .WithIdentity("DbStatisticsJob", "DbStatisticsGroup")
                    .Build();

                //job.JobDataMap.Add("Provider", serviceProvider);

                //创建触发器
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("DbStatisticsJob_Trigger", "DbStatisticsGroup")
                    .StartNow()
                 //   .WithSimpleSchedule(x => x
                 //.WithIntervalInSeconds(15).RepeatForever())
                 .WithCronSchedule("0 0 0 1/1 * ?")//每天00:00:00触发 0 0 0 1/1 * ?
                                                   //.WithCronSchedule("0 0/1 * * * ?")
                 .Build();
                //调度器添加任务
                Scheduler.ScheduleJob(job, trigger).Wait();
                #endregion

                #region 1分钟时间粒度
                var Job1Min = JobBuilder.Create<IDataGroupBy1MinJob>()
                    .WithIdentity("Job1Min", "Job1MinGroup")
                    .Build();
                var trigger1Min = TriggerBuilder.Create()
                    .WithIdentity("Job1Min_Tigger", "Job1MinGroup")
                    .StartNow().WithCronSchedule("0 0/1 * * * ?").Build();//每分钟触发
                                                                           //调度器添加任务
                Scheduler.ScheduleJob(Job1Min, trigger1Min).Wait();
                #endregion

                #region 5分钟时间粒度
                var Job5Min = JobBuilder.Create<IDataGroupBy5MinJob>()
                    .WithIdentity("Job5Min", "Job5MinGroup")
                    .Build();
                var trigger5Min = TriggerBuilder.Create()
                    .WithIdentity("Job5Min_Tigger", "Job5MinGroup")
                    .StartNow().WithCronSchedule("0 0/5 * * * ?").Build();//每5分钟触发
                                                                          //调度器添加任务
                Scheduler.ScheduleJob(Job5Min, trigger5Min).Wait();
                #endregion

                #region 10分钟时间粒度
                var Job10Min = JobBuilder.Create<IDataGroupBy10MinJob>()
                   .WithIdentity("Job10Min", "Job10MinGroup")
                   .Build();
                var trigger10Min = TriggerBuilder.Create()
                    .WithIdentity("Job10Min_Tigger", "Job10MinGroup")
                    .StartNow().WithCronSchedule("0 0/10 * * * ?").Build();//每10分钟触发
                                                                          //调度器添加任务
                Scheduler.ScheduleJob(Job10Min, trigger10Min).Wait();
                #endregion

                #region 小时时间粒度
                var Job1Hour = JobBuilder.Create<IDataGroupBy1HourJob>()
                    .WithIdentity("Job1Hour", "Job1HourGroup")
                    .Build();
                var trigger1Hour = TriggerBuilder.Create()
                    .WithIdentity("Job1Hour_Tigger", "Job1HourGroup")
                    .StartNow().WithCronSchedule("0 0 0/1 * * ? *").Build();//每小时触发
                                                                           //调度器添加任务
                Scheduler.ScheduleJob(Job1Hour, trigger1Hour).Wait();
                #endregion

                #region redis过期数据
                var JobRidesData = JobBuilder.Create<IRidesDataJob>()
                      .WithIdentity("JobRidesData", "JobRidesDataGroup")
                      .Build();
                var triggerRidesData = TriggerBuilder.Create()
                    .WithIdentity("JobRidesData_Tigger", "JobRidesDataGroup")
                    .StartNow().WithCronSchedule("0 0/20 * * * ?").Build();//每20分钟触发
                                                                           //调度器添加任务
                Scheduler.ScheduleJob(JobRidesData, triggerRidesData).Wait();
                #endregion
                _logger.LogInformation("LoadScheduleJob:初始化加载任务成功");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, "初始化加载任务失败");
            }
        }
        /// <summary>
        /// 结束调度
        /// </summary>
        public void EndScheduler()
        {
            try
            {
                if (Scheduler == null)
                {
                    return;
                }

                if (Scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
                    Scheduler = null;

                _logger.LogInformation("Schedule job upload as application stopped");
            }
            catch (Exception e)
            {
                _logger.LogError($"EndScheduler{e}", "关闭调度器失败");
            }
        }
    }
}
