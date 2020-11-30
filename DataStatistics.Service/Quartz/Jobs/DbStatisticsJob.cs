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
        public DbStatisticsJob(ILogger<DbStatisticsJob> logger, IEasyCachingProviderFactory providerFactory, IMJLogOtherRepository repository)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            _repository = repository;
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
                List<string> keys = redisProvider.SearchKeys("*", 0).Where(i => !i.Contains("r")).Where(i => {
                    return Regex.IsMatch(i, "^\\d+$");
                }).ToList();
                foreach (var key in keys)
                {
                    var length = redisProvider.LLen(key);
                    var data = redisProvider.LRange<UserActionModel>(key, 0, length).Where(i => i.date >= startTime && i.date < endtTime).ToList();
                    ////获取版本号
                    //List<string> vList = _repository.GetVersion(Convert.ToInt32(key));
                    //获取类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();
                 
                    foreach (var type in dataType)
                    {
                        var udata = data.Where(i=>i.type==type).ToList();
                        ////分版本号
                        //foreach (var v in vList)
                        //{
                        //    var vdata = udata.Where(i => i.version == v).ToList();
                        //    var list=GetList(key,type,v,vdata,startTime);
                        //    var res = _repository.Insert(list);
                        //    _logger.LogInformation($"更新:{res}条数据,类型:{type},版本号:{v},时间:{DateTime.Now:yyyy-MMM-dd HH:mm:ss:ffff}");
                        //}
                        //所有版本
                        var all = GetList(key, type, udata, startTime);
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
        public List<OverallSituationModel> GetList(string key,int type,List<UserActionModel> vdata,DateTime startTime)
        {
            List<OverallSituationModel> list = new List<OverallSituationModel>();
            //all
            OverallSituationModel all = new OverallSituationModel()
            {
                areaid = Convert.ToInt32(key),
                activeUsers = vdata.Where(i => i.uid != 0).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0).Count(),
                platForm = PlatFromEnum.All.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(all);
            //windows
            OverallSituationModel windows = new OverallSituationModel()
            {
                areaid = Convert.ToInt32(key),
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm == PlatFromEnum.Windows.GetName()).GroupBy(i=>i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm == PlatFromEnum.Windows.GetName()).Count(),
                platForm = PlatFromEnum.Windows.GetName(),
                dataTime = startTime,
                type=type
            };
            list.Add(windows);
            //ios
            OverallSituationModel ios = new OverallSituationModel()
            {
                areaid = Convert.ToInt32(key),
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm == PlatFromEnum.IOS.GetName()).GroupBy(i => i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm == PlatFromEnum.IOS.GetName()).Count(),
                platForm = PlatFromEnum.IOS.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(ios);
            //android
            OverallSituationModel android = new OverallSituationModel()
            {
                areaid = Convert.ToInt32(key),
                activeUsers = vdata.Where(i => i.uid != 0 && i.platForm == PlatFromEnum.Android.GetName()).GroupBy(i => i.uid).Count(),
                registeredUsers = vdata.Where(i => i.uid == 0 && i.platForm == PlatFromEnum.Android.GetName()).Count(),
                platForm = PlatFromEnum.Android.GetName(),
                dataTime = startTime,
                type = type
            };
            list.Add(android);
            return list;
        }
    }
}
