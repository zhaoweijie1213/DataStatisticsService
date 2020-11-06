using DataStatistics.Model.mj_log_other;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Repositorys
{
    public interface IMJLogOtherRepository : IBaseRepository
    {
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <returns></returns>
        List<UserActionModel> GetUserActions();
    }
}
