using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface IDataService
    {
  
        ///// <summary>
        ///// 昨日概况
        ///// </summary>
        ///// <returns></returns>
        List<OverallSituationModel> DataSituationForYestoday(int areaid, int type);
        /// <summary>
        /// 近期趋势
        /// </summary>
        /// <returns></returns>
        DaysDataModel ThirtyDaysData(int areaid, int type, DateTime time);
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type">0秒,1分,2时</param>
        /// <param name="value"></param>
        /// <returns></returns>
        DaysDataModel RealTimeData(int areaid, int value, int type, string version);
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        AreaParamsModel GetAreaParams(int areaid,int type);
        /// <summary>
        /// 单场景分析
        /// </summary>
        /// <param name="areaid">区域id</param>
        /// <param name="days">天数</param>
        /// <param name="platFrom">平台</param>
        /// <param name="otherParam">其它参数</param>
        /// <param name="dateRange">日期范围</param>
        /// <returns></returns>
        SingleSceneModel GetSingleSceneData(int areaid, int days, string platFrom, string otherParam, string dateRange, int type, string version);
        /// <summary>
        /// 用户画像
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="strat"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        UserPicModel GetUserPic(int areaid, DateTime start, DateTime end, int type, string version);
        /// <summary>
        /// 漏斗图数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="platForm"></param>
        /// <param name="day"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="other"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        FunnelDataModel GetFunnelData(int areaid, string platForm, int days, DateTime? start, DateTime? end, string other, string otherValue, int type, string version);
    }
}
