using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.DataProcessing;
using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Services.DataProcessingl.Impl
{
    public class DataProcessing: ICapSubscribe, IDataProcessing
    {
        private readonly ICacheManage _cache;

        private readonly IMJLogOtherRepository _repository;

        private readonly ILogger<DataProcessing> _logger;

        public DataProcessing(ICacheManage cache, IMJLogOtherRepository repository, ILogger<DataProcessing> logger)
        {
            _cache = cache;
            _repository = repository;
            _logger = logger;
            //CapSubscribe = _Configuration["CAP:PublishName"];
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="model"></param>
        [CapSubscribe("user_action_cap_1")]
        public void SubscribeWithnoController(ReciveUserActionModel model)
        {
            try
            {
                List<UserActionModel> list = new List<UserActionModel>();
                _logger.LogDebug($"SubscribeWithnoController:收到数据向Redis添加:{model.areaid}大厅,{model.type}类型,{model.platForm}平台,{model.version}版本,时间:{model.date},系统时间{DateTime.Now}");
                //向rides list添加元素
                DateTime time = model.date;
                //获取uuid
                string config = model.data;
                JObject jo = JsonConvert.DeserializeObject<JObject>(config);
                string uuid = " ";
                if (jo.Property("UUID")!=null)
                {
                    uuid = jo["UUID"].ToString();
                }
                
                UserActionModel user = new UserActionModel()
                { 
                    id=model.id,
                    areaid=model.areaid,
                    date=model.date,
                    uid=model.uid,
                    version=model.version,
                    uuid=uuid,
                    platForm=model.platForm,
                    type=model.type
                };
                string ktime = "";
                if (time.Minute>0)
                {
                    ktime = time.AddHours(1).ToString("yyyyMMddHH");
                }
                else
                {
                    ktime = time.ToString("yyyyMMddHH");
                }
                list.Add(user);
                _cache._redisProvider.RPush($"{model.areaid}_t{ktime}", list);
                Random rand = new Random();
                //7天保存
                int second = 7*((int)KeyExpireTime.realData);
                _cache._redisProvider.KeyExpire($"{model.areaid}_t{ktime}", second);

                var checkAct = _repository.QueryByContion(model.areaid, model.uid, model.type, model.platForm,model.version, uuid);
                if (checkAct==null)
                {
                    UserActionStatisticsModel actStat = new UserActionStatisticsModel()
                    {
                        areaid = model.areaid,
                        type = model.type,
                        platForm = model.platForm==null?" ":model.platForm,
                        uid = model.uid,
                        uuid = uuid,
                        added = model.date,
                        version=model.version==null?" ":model.version
                    };
                    List<UserActionStatisticsModel> stlist = new List<UserActionStatisticsModel>();
                    if (!string.IsNullOrEmpty(uuid)||actStat.uid!=0)
                    {
                        stlist.Add(actStat);
                        _repository.Insert(stlist);
                    }
                }
                //List<string> keys = new List<string>();
                //string configKeys;
                //foreach (var item in jo)
                //{
                //    keys.Add(item.Key);
                //}
                //configKeys = string.Join(",", keys);
                //var areaParams = _repository.GetAreaParams(model.areaid, model.type);
                //if (areaParams != null)
                //{

                //    if (configKeys.Equals(areaParams.configKeys))
                //    {
                //        return;
                //    }
                //    var akeys = areaParams.configKeys.Split(',');
                //    keys.Concat(akeys).ToList().Distinct();
                //    areaParams.configKeys = string.Join(",", keys);
                //    _repository.UpdateAreaParams(areaParams);
                //}
                //else
                //{
                //    _repository.Insert(new List<AreaParamsModel>() {
                //     new AreaParamsModel()
                //     {
                //        areaid=model.areaid,
                //        configKeys=configKeys,
                //        type=model.type
                //     }
                //    });
                //}
                #region 检查版本号
                //_logger.LogInformation($"SubscribeWithnoController:检查版本号");
                var check = _repository.CheckAreaVersion(model.areaid,model.version.Trim());
                List<AreaVersion> vs = new List<AreaVersion>() { new AreaVersion { areaid = model.areaid, version = model.version.Trim() } };
                if (check.Count==0)
                {
                    
                    var res = _repository.Insert(vs);
                }
                if (check.Count>1)
                {
                    _repository.Delete(check);
                    _repository.Insert(vs);
                }
                #endregion
            }
            catch (Exception e)
            {
                _logger.LogError($"SubscribeWithnoController:{e.Message}");
            }
          
        }
    }
}
