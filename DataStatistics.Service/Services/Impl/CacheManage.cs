using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services;
using EasyCaching.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStatistics.Service.Services.Impl
{
    public class CacheManage : ICacheManage
    {
        /// <summary>
        /// 日志
        /// </summary>
        private ILogger<CacheManage> _logger;
        /// <summary>
        /// rides
        /// </summary>
        readonly IEasyCachingProviderFactory _factory;
        /// <summary>
        /// rides缓存
        /// </summary>
        public IRedisCachingProvider _redisProvider { get; set; }
        /// <summary>
        /// 30天原始数据
        /// </summary>
        public List<UserActionModel> rawDataForThirtyDays { get; set; }
        /// <summary>
        /// 本地缓存
        /// </summary>
        public IMemoryCache  _memoryCache { get; set; }
        private readonly IMJLogOtherRepository _repository;
        public CacheManage(IEasyCachingProviderFactory factory, IMemoryCache memoryCache,ILogger<CacheManage> logger, IMJLogOtherRepository repository)
        {
            _logger = logger;
            _factory = factory;
            _redisProvider = _factory.GetRedisProvider("userAction");
            _memoryCache = memoryCache;
            _repository = repository;
        }

        /// <summary>
        /// 30天数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetRawDataForThirty()
        {
            try
            {
                List<UserActionModel> data = new List<UserActionModel>();
                var res = _memoryCache.TryGetValue("rawDataForThirtyDays", out data);
                if (res)
                {
                    return data;
                }
                else
                {
                    var end = DateTime.Now.Date;
                    var start = DateTime.Now.Date.AddDays(-30);
                    data = _repository.GetUserActions(start, end);
                    _memoryCache.Set("rawDataForThirtyDays",data,TimeSpan.FromHours(24));
                }
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRawDataForThirty:{e.Message}");
                throw;
            }
           
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
