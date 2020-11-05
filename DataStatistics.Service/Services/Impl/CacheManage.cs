using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Services;
using EasyCaching.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataStatistics.Service.Services.Impl
{
    public class CacheManage : ICacheManage
    {
        IEasyCachingProviderFactory _factory;
        public CacheManage(IEasyCachingProviderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SetUserAction(List<UserActionModel> list)
        {
            //return true;
            string lists = JsonConvert.SerializeObject(list);
            var provider = _factory.GetRedisProvider("myredisname");
            provider.StringSet("100", lists);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<UserActionModel> GetUserAction()
        {
            var provider = _factory.GetRedisProvider("myredisname");
            var res=provider.StringGet("100");
            List<UserActionModel> models = JsonConvert.DeserializeObject<List<UserActionModel>>(res);
            return models;
        }
    }
}
