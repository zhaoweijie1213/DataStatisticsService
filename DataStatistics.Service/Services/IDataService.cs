using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
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
        ///// <summary>
        ///// 昨日概况
        ///// </summary>
        ///// <returns></returns>
        List<OverallSituationModel> DataSituationForYestoday(int areaid);
        /// <summary>
        /// 近期趋势
        /// </summary>
        /// <returns></returns>
        ThirtyDaysDataModel ThirtyDaysData(int areaid, DateTime time);
    }
}
