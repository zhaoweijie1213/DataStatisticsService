using DataStatistics.Model.mj_log_other;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        long Insert<T>(List<T> list);
        /// <summary>
        /// 获取概况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        List<OverallSituationModel> GetSituation(int areaid);
        /// <summary>
        /// 近期情况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        List<OverallSituationModel> GetThirtyDaysData(int areaid, DateTime time);
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        AreaParamsModel GetAreaParams(int areaid);
    }
}
