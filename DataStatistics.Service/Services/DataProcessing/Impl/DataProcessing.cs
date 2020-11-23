using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.DataProcessing;
using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Memory;
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
        /// <summary>
        /// 
        /// </summary>
        private readonly IMJLogOtherRepository _repository;
        public DataProcessing(ICacheManage cache, IMJLogOtherRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="model"></param>
        [CapSubscribe("Data.Recive")]
        public void SubscribeWithnoController(UserActionModel model)
        {
            List<UserActionModel> list = new List<UserActionModel>();
            list.Add(model);
            //向rides list添加元素
            long length = _cache._redisProvider.RPush(model.areaid.ToString(), list);
            //大厅参数
            string config = model.data;
            JObject jo = JsonConvert.DeserializeObject<JObject>(config);
            List<string> keys = new List<string>();
            string configKeys = "";
            foreach (var item in jo)
            {
                keys.Add(item.Key);
            }
            configKeys = string.Join(",", keys);
            var areaParams = _repository.GetAreaParams(model.areaid);
            if (areaParams != null)
            {
   
                if (configKeys.Equals(areaParams.configKeys))
                {
                    return;
                }
                var akeys = areaParams.configKeys.Split(',');
                keys.Concat(akeys).ToList().Distinct();
                //configKeys =;
                areaParams.configKeys = string.Join(",", keys);
                var res = _repository.UpdateAreaParams(areaParams);
            }
            else
            {
                var res = _repository.Insert(new List<AreaParamsModel>() {
                 new AreaParamsModel()
                 {
                    areaid=model.areaid,
                    configKeys=configKeys
                 }
                });
            }
        }
    }
}
