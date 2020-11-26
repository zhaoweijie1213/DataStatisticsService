using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Quartz
{

    public interface IQuartzManager
    {
        /// <summary>
        /// 初始化加载调度任务
        /// </summary>
        void LoadScheduleJob(IServiceProvider serviceProvider);
        /// <summary>
        /// 结束调度
        /// </summary>
        void EndScheduler();
    }
}
