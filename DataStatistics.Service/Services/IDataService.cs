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
        DaysDataModel ThirtyDaysData(int areaid, DateTime time);
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type">0秒,1分,2时</param>
        /// <param name="value"></param>
        /// <returns></returns>
        DaysDataModel RealTimeData(int areaid, int type, int value);
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        AreaParamsModel GetAreaParams(int areaid);
    }
}
