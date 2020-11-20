using DataStatistics.Model.mj_log_other;
using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface ICacheManage
    {
        /// <summary>
        /// redis
        /// </summary>
        IRedisCachingProvider _redisProvider { get; set; }
        /// <summary>
        /// 30天数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetRawDataForThirty();
        /// <summary>
        /// 获取整个列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> GetAllList<T>(string key);
    }
}
