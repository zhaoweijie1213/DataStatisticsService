using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Enums;
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
        private readonly ILogger<CacheManage> _logger;
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
        private readonly IMJLog3Repository _mjlog3repository;
        public CacheManage(IEasyCachingProviderFactory factory, IMemoryCache memoryCache,ILogger<CacheManage> logger, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository)
        {
            _logger = logger;
            _factory = factory;
            _redisProvider = _factory.GetRedisProvider("userAction");
            _memoryCache = memoryCache;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
        }

        /// <summary>
        /// 30天数据
        /// 只留7天数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetRawDataForThirty(string areaid,DateTime start,DateTime end)
        {
            try
            {
                List<UserActionModel> data = new List<UserActionModel>();
                var res = _memoryCache.TryGetValue(areaid, out data);
                if (res)
                {
                    return data.ToList();
                }
                else
                {
                    int hourCount = (int)((end - start).TotalHours);
                    for (int i = 1; i <= hourCount; i++)
                    {
                        var ktime = start.AddHours(i).ToString("yyyyMMddHH");
                        long length = _redisProvider.LLen($"{areaid}_t{ktime}");
                        var idate = _redisProvider.LRange<UserActionModel>($"{areaid}_t{ktime}", 0, length);
                        if (i==1)
                        {
                            data = idate;
                        }
                        else
                        {
                            data.AddRange(idate);
                        }
                    }
                    //var end = DateTime.Now;
                    //var start = DateTime.Now.Date.AddDays(-30);
                    //data = _repository.GetUserActions(start, end);
                    //var areaids = data.GroupBy(i => i.areaid).Select(i=>i.Key).ToList();
                    //List<int> gameids = _mjlog3repository.GetGameid();
                    //foreach (var item in gameids)
                    //{
                        //int key = item * 100;
                        //固定每天00:00点过期
                        //var sdata = data.Where(i => i.areaid == item).ToList();
                        //long length = _redisProvider.LLen(areaid.ToString());
                        //data = _redisProvider.LRange<UserActionModel>(areaid.ToString(), 0, length);
                        _memoryCache.Set(areaid.ToString(), data, DateTimeOffset.Now.Date.AddDays(1));
                    //}
                  
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


        /// <summary>
        /// data插入redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public bool Rpush(int key,List<UserActionModel> list,DateTime start,DateTime end)
        {
            try
            {
                TimeSpan time = end - start;
                int hourCount = (int)time.TotalHours;
                for (int i = 0; i < hourCount; i++)
                {
                    var kstart = start.AddHours(i-1);
                    var kend = start.AddHours(i);
                    var tkey = kend.ToString("yyyyMMddHH");
                    _redisProvider.KeyDel($"{key}_t{tkey}");
                    var data = list.Where(i => i.date > kstart && i.date <= kend).ToList();
                    if (data.Count>0)
                    {
                        _redisProvider.RPush($"{key}_t{tkey}",data );
                        _redisProvider.KeyExpire($"{key}_t{tkey}", i*(int)KeyExpireTime.unitData);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Rpush:{e.Message}");
                return false;
            }   
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
           
        }
    }
}
