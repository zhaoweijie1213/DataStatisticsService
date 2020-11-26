using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Quartz.Impl
{
    /// <summary>
    /// 自定义JobFactory
    /// </summary>
    public class IOCJobFactory : IJobFactory
    {
        protected readonly IServiceProvider Container;
        public IOCJobFactory(IServiceProvider container)
        {
            Container = container;
        }
        /// <summary>
        /// 在触发触发器时由调度程序调用,生成调用execute的job实例
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var res = Container.GetService(bundle.JobDetail.JobType) as IJob;
                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
          
        }

        /// <summary>
        /// Allows the job factory to destroy/cleanup the job if needed.
        /// </summary>
        /// <param name="job"></param>
        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
