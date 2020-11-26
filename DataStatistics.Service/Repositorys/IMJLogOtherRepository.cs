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
        List<UserActionModel> GetUserActions(DateTime start, DateTime end);
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
        List<OverallSituationModel> GetSituation(int areaid, int type);
        /// <summary>
        /// 近期情况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        List<OverallSituationModel> GetThirtyDaysData(int areaid, int type, DateTime time);
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        AreaParamsModel GetAreaParams(int areaid,int type);
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<string> GetVersion(int areaid);
        /// <summary>
        /// user action查询
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<UserActionModel> GetActionData(int areaid, string condition);
        /// <summary>
        /// 修改大厅参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        bool UpdateAreaParams(AreaParamsModel list);
    }
}
