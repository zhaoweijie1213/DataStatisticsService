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
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<UserActionModel> GetUserAction()
        {
            var data= _redisProvider.SMembers<UserActionModel>("1200");
            List<UserActionModel> models = data;
            return models;
        }
    }
}
