  using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Quartz.Jobs.Interface;
using DataStatistics.Service.Repositorys;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Impl
{
    /// <summary>
    /// 数据处理任务
    /// </summary>
    public class DbStatisticsJob : IDbStatisticsJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DbStatisticsJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMJLogOtherRepository _repository;
        private readonly IMJLog3Repository _mjlog3repository;
        public DbStatisticsJob(ILogger<DbStatisticsJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime endtTime = DateTime.Now.Date;
                DateTime startTime = DateTime.Now.Date.AddDays(-1);
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                #region 昨日概况总结
                //获取所有的key
                List<int> gameids = _mjlog3repository.GetGameid();
                foreach (var item in gameids)
                {
                    string timekey = endtTime.ToString("yyyyMMdd");
                    int areaid = item * 100;
                    List<string> keys = redisProvider.SearchKeys($"{areaid}_t{timekey}*", 0);
                    List<UserActionModel> data = new List<UserActionModel>();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        var length = redisProvider.LLen(keys[i]);
                        var udata = redisProvider.LRange<UserActionModel>(keys[i], 0, length);
                        if (i==0)
                        {
                            data = udata;
                        }
                        else
                        {
                            data.AddRange(udata);
                        }
                    }
                    //foreach (var key in keys)
                    //{
                    //    var length = redisProvider.LLen(key);
                    //    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();
                    //}
                    //获取平台类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();
                    foreach (var type in dataType)
                    {
                        var udata = data.Where(i => i.type == type).ToList();
                        //所有版本
                        var all = GetList(areaid, type, udata, startTime);
                        var ares = _repository.Insert(all);
                        _logger.LogInformation($"更新:{ares}条数据,类型:{type},时间:{DateTime.Now:yyyy-MMM-dd HH:mm:ss:ffff}");
                    }
                }
            
                #endregion

            }
            catch (Exception e)
            {
                _logger.LogError($"Execute:{e.Message}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="vdata"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<OverallSituationModel> GetList(int key,int type,List<UserActionModel> vdata,DateTime startTime)
        {
            List<OverallSituationModel> list = new List<OverallSituationModel>();
            //all
            OverallSituationModel all = new OverallSituationModel()
            {
                areaid = key,
                activeUsers = vdata.Where(i => i.uid != 0).GroupBy(i=>i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0).Count(),
                platForm = PlatFromEnum.All.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(all);
            //windows
            OverallSituationModel windows = new OverallSituationModel()
            {
                areaid = key,
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm.ToLower() == PlatFromEnum.Windows.GetName().ToLower()).GroupBy(i=>i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm.ToLower() == PlatFromEnum.Windows.GetName().ToLower()).Count(),
                platForm = PlatFromEnum.Windows.GetName(),
                dataTime = startTime,
                type=type
            };
            list.Add(windows);
            //ios
            OverallSituationModel ios = new OverallSituationModel()
            {
                areaid = key,
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm.ToLower() == PlatFromEnum.IOS.GetName().ToLower()).GroupBy(i => i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm.ToLower() == PlatFromEnum.IOS.GetName().ToLower()).Count(),
                platForm = PlatFromEnum.IOS.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(ios);
            //android
            OverallSituationModel android = new OverallSituationModel()
            {
                areaid = key,
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm.ToLower() == PlatFromEnum.Android.GetName().ToLower()).GroupBy(i => i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm.ToLower() == PlatFromEnum.Android.GetName().ToLower()).Count(),
                platForm = PlatFromEnum.Android.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(android);
            return list;
        }
    }
}
