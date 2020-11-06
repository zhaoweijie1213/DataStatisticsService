using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Quartz.Impl
{
    public class IOCJobFactory : IJobFactory
    {
        protected readonly IServiceProvider Container;
        public IOCJobFactory(IServiceProvider container)
        {
            Container = container;
        }
        /// <summary>
        /// Called by the scheduler at the time of the trigger firing, in order to produce
        /// a Quartz.IJob instance on which to call Execute.
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
