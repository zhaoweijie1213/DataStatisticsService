using DataStatistics.Service.Repositorys;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Quartz.Impl
{
    public class QuartzManager : IQuartzManager
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<QuartzManager> _logger;
        public IScheduler Scheduler { get; set; }

        public QuartzManager(ILogger<QuartzManager> logger, IServiceProvider IocContainer)
        {
            _logger = logger;
            var schedulerFactory = new StdSchedulerFactory();
            IOCJobFactory iocJobfactory = new IOCJobFactory(IocContainer);
            Scheduler = schedulerFactory.GetScheduler().Result;
            Scheduler.JobFactory = iocJobfactory;
            Scheduler.Start().Wait();
        }
        /// <summary>
        /// 初始化加载调度任务
        /// </summary>
        public void LoadScheduleJob(IServiceProvider serviceProvider)
        {
            try
            {

                //创建任务
                var job = JobBuilder.Create<IJob>()
                    .WithIdentity("DbStatisticsJob", "DbStatisticsGroup")
                    .Build();

                //job.JobDataMap.Add("Provider", serviceProvider);

                //创建触发器
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("DbStatisticsJob_Trigger", "DbStatisticsGroup")
                    .StartNow()
                 //   .WithSimpleSchedule(x => x
                 //.WithIntervalInSeconds(15).RepeatForever())
                 .WithCronSchedule("0 0/1 * * * ? *")//0 0 6 ? * *每天六点触发
                 .Build();
                //调度器添加任务
                Scheduler.ScheduleJob(job, trigger).Wait();
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
