using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.Impl
{
    public class LoadDataService : ILoadDataService
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<LoadDataService> _logger;
        /// <summary>
        /// 缓存
        /// </summary>
        readonly IEasyCachingProviderFactory _factory;
        readonly ICacheManage _cache;

        private readonly IMJLogOtherRepository _repository;
        private readonly IMJLog3Repository _mjlog3repository;

        public LoadDataService(ILogger<LoadDataService> logger, IServiceProvider IocContainer, IEasyCachingProviderFactory factory, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository, ICacheManage cache)
        {
            _logger = logger;
            _factory = factory;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
            _cache = cache;
        }
        /// <summary>
        /// 加载版本号 
        /// </summary>
        public void LoadVersion()
        {
            try
            {
                _logger.LogInformation("----初始化版本号----");
                _repository.DeleteAllAreaVersion();
                List<int> gameids = _mjlog3repository.GetGameid();
                var redisProvider = _factory.GetRedisProvider("userAction");
                foreach (var item in gameids)
                {
                    var areaid = item * 100;
                    List<AreaVersion> list = new List<AreaVersion>();
                    var vlist = _repository.GetVersion(areaid);
                    foreach (var v in vlist)
                    {
                        list.Add(new AreaVersion { areaid = areaid, version = v });
                    }
                    if (list.Count > 0)
                    {
                        var count = _repository.Insert(list);
                        _logger.LogInformation($"----{areaid}大厅版本号初始化成功----");
                    }
                }
                _logger.LogInformation("----初始化版本号结束----");
            }
            catch (Exception e)
            {
                _logger.LogError($"LoadVersion{e.Message}", "初始化版本号失败");
            }
        }
        /// <summary>
        /// 加载30天数据
        /// </summary>
        public void LoadThirtyUserAction()
        {
            try
            {
                _logger.LogInformation("----加载30天数据----");
                List<int> gameids = _mjlog3repository.GetGameid();
                var end = DateTime.Now.Date;
                //var start = DateTime.Now.Date.AddDays(-30);
                var start = DateTime.Now.Date.AddDays(-7);
                //var data = _repository.GetUserActions(start, end);

                var redisProvider = _factory.GetRedisProvider("userAction");
                foreach (var item in gameids)
                {
                    int areaid = item * 100;
                    string sql = $" and date between '{start}' and '{end}' ";
                    var sdata = _repository.GetActionData(areaid, sql);
                    //var sdata = data.Where(i => i.areaid == areaid).ToList();
                    if (sdata.Count > 0)
                    {
                        var count = _cache.Rpush(areaid, sdata, start, end);
                        _logger.LogInformation($"----加载{areaid}大厅数据成功,共{sdata.Count}条----");
                    }
                }

                _logger.LogInformation("----加载数据完成----");
            }
            catch (Exception e)
            {

                _logger.LogError($"LoadThirtyUserAction{e.Message}", "加载数据失败");
            }

        }

        public void Dispose()
        {
            //base.Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
    }
}
