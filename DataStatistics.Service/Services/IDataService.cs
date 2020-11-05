using DataStatistics.Model.mj_log_other;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface IDataService
    {
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <returns></returns>
        List<UserActionModel> GetUserActions();
    }
}
