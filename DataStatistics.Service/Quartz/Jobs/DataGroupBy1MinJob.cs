using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Quartz.Jobs.Interface;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.Common;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStatistics.Service.Quartz.Jobs
{
    public class DataGroupBy1MinJob : IDataGroupBy1MinJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<DataGroupBy1MinJob> _logger;
        /// <summary>
        /// easycaching
        /// </summary>
        private readonly IEasyCachingProviderFactory _providerFactory;

        private readonly IMJLogOtherRepository _repository;

        private readonly IMJLog3Repository _mjlog3repository;
        public DataGroupBy1MinJob(IEasyCachingProviderFactory providerFactory, ILogger<DataGroupBy1MinJob> logger, IMJLogOtherRepository repository, IMJLog3Repository mjlog3repository)
        {
            _providerFactory = providerFactory;
            _logger = logger;
            _repository = repository;
            _mjlog3repository = mjlog3repository;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var time = DateTime.Now.AddMinutes(-5);
                DateTime endtTime = Convert.ToDateTime(time.ToString("yyyy-MM-dd HH:mm:00"));
                DateTime startTime = endtTime.AddMinutes(-1);
                string ketTime = "";
                if (time.Minute > 0)
                {
                    ketTime = time.AddHours(1).ToString("yyyyMMddHH");
                }
                else
                {
                    ketTime = time.ToString("yyyyMMddHH");
                }
                var redisProvider = _providerFactory.GetRedisProvider("userAction");
                //获取所有的key
                List<int> gameids = _mjlog3repository.GetGameid();
                foreach (var item in gameids)
                {
                    int key = item * 100;
                    var length = redisProvider.LLen($"{key}_t{ketTime}");
                    //获取一个小时内所有数据
                    var all_data = redisProvider.LRange<UserActionModel>($"{key}_t{ketTime}", 0, length);
                    all_data=all_data.Where(i => i.date > startTime && i.date <= endtTime).ToList();
                    //获取版本号
                    List<string> vList = _repository.GetAreaVersion(Convert.ToInt32(key)).Select(i => i.version).ToList();
                    //获取类型
                    List<int> dataType = PlatFromEnumExt.GetEnumAllValue<DataType>();
                    foreach (var type in dataType)
                    {
                        var tdata = all_data.Where(i => i.type == type).ToList();
                        //所有版本
                        List<JobRealData> areg = JobDataProcessing.GetDataList(tdata, "", endtTime);
                        redisProvider.RPush($"r_1_{type}_{key}", areg);
                        _logger.LogInformation($"统计数据{JsonConvert.SerializeObject(areg)}");
                        foreach (var v in vList)
                        {
                            var data = tdata.Where(i => i.version == v).ToList();
                            List<JobRealData> reg = JobDataProcessing.GetDataList(data,v, endtTime);
                            //1分钟时间粒度 分版本
                            redisProvider.RPush($"r_1_{type}_{v}_{key}", reg);
                            //redisProvider.KeyExpire($"r_1_{type}_{v}_{key}", (int)KeyExpireTime.realData);
                            _logger.LogInformation($"统计数据{v}版本{JsonConvert.SerializeObject(reg)}");
                        }
                        //redisProvider.KeyExpire($"r_1_{type}_{key}", (int)KeyExpireTime.realData);
                        _logger.LogInformation($"{key}大厅,1分钟时间粒度,类别:{type}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Execute:{e.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
