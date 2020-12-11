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
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        bool Delete<T>(List<T> list);
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
        /// <summary>
        /// 大厅版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        List<AreaVersion> GetAreaVersion(int areaid);
        /// <summary>
        /// 删除大厅版本
        /// </summary>
        /// <returns></returns>
        bool DeleteAllAreaVersion();
        /// <summary>
        /// 检查是否存在该版本
        /// </summary>
        /// <returns></returns>
        List<AreaVersion> CheckAreaVersion(int areaid, string version);
        /// <summary>
        /// 查询当天行为统计
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <param name="platForm"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        UserActionStatisticsModel QueryByContion(int areaid,int uid, int type, string platForm, string version,string uuid);
        /// <summary>
        /// 日行为分析列表
        /// </summary>
        /// <returns></returns>
        List<UserActionStatisticsModel> QueryUserActStat(int areaid, DateTime start, DateTime end);
        /// <summary>
        /// 删除日行为分析数据
        /// </summary>
        /// <returns></returns>
        bool DeleteUserActStat(DateTime end);
        /// <summary>
        /// 删除概况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool DeleteYeatodayData(int areaid, DateTime time);
    }
}
