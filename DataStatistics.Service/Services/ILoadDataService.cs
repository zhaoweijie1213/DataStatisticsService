using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface ILoadDataService:IDisposable
    {
        /// <summary>
        /// 加载版本号 
        /// </summary>
        void LoadVersion();
        /// <summary>
        /// 加载30天数据
        /// </summary>
        void LoadThirtyUserAction();
    }
}
