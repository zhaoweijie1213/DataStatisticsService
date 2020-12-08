using DataStatistics.Model.mj_log_other;
using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface ICacheManage: IDisposable
    {
        /// <summary>
        /// redis
        /// </summary>
        IRedisCachingProvider _redisProvider { get; set; }
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <returns></returns>
        List<UserActionModel> GetRawDataForThirty(string areaid, DateTime start, DateTime end);
        /// <summary>
        /// 获取整个列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> GetAllList<T>(string key);
        /// <summary>
        /// 插入redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        bool Rpush(int key, List<UserActionModel> list, DateTime start, DateTime end);

    }
}
