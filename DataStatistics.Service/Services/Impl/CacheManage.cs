using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Services;
using EasyCaching.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStatistics.Service.Services.Impl
{
    public class CacheManage : ICacheManage
    {
        readonly IEasyCachingProviderFactory _factory;
        public IRedisCachingProvider _redisProvider { get; set; }
        public CacheManage(IEasyCachingProviderFactory factory)
        {
            _factory = factory;
            _redisProvider = _factory.GetRedisProvider("userAction");
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SetUserAction(List<UserActionModel> list)
        {
            //return true;
            //var provider = _factory.GetCachingProvider("myredisname");
            var areaids = list.GroupBy(i=>i.areaid).Select(i=>i.Key).ToList();
            foreach (var areaid in areaids)
            {
                var data = list.Where(i=>i.areaid==areaid).ToList();
                var len = _redisProvider.SAdd(areaid.ToString(),data);
            }
            string lists = JsonConvert.SerializeObject(list);
            return true;
        }
        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<UserActionModel> GetUserAction(int areaid)
        {
            var data= _redisProvider.SMembers<UserActionModel>(areaid.ToString());
            List<UserActionModel> models = data;
            return models;
        }

        /// <summary>
        /// 获取整个列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetAllList<T>(string key)
        {
            var length = _redisProvider.LLen(key);
            var data = _redisProvider.LRange<T>(key,0,length-1);
            return data;
        }
    }
}
