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
        /// 存入缓存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool SetUserAction(List<UserActionModel> list);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<UserActionModel> GetUserAction();
    }
}
