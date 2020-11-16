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
        /// <summary>
        /// 单场景分析
        /// </summary>
        /// <param name="areaid">区域id</param>
        /// <param name="days">天数</param>
        /// <param name="platFrom">平台</param>
        /// <param name="otherParam">其它参数</param>
        /// <param name="dateRange">日期范围</param>
        /// <returns></returns>
        SingleSceneModel GetSingleSceneData(int areaid, int days, string platFrom, string otherParam, string dateRange);
        /// <summary>
        /// 用户画像
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="strat"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        UserPicModel GetUserPic(int areaid, DateTime start, DateTime end);
    }
}
