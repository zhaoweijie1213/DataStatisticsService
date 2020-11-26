using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.DataProcessing;
using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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

        public DataProcessing(ICacheManage cache, IMJLogOtherRepository repository)
        {
            _cache = cache;
            _repository = repository;
            //CapSubscribe = _Configuration["CAP:PublishName"];
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="model"></param>
        [CapSubscribe("user_action_cap_1")]
        public void SubscribeWithnoController(UserActionModel model)
        {
            List<UserActionModel> list = new List<UserActionModel>
            {
                model
            };
            //向rides list添加元素
            _cache._redisProvider.RPush(model.areaid.ToString(), list);
            //大厅参数
            string config = model.data;
            JObject jo = JsonConvert.DeserializeObject<JObject>(config);
            List<string> keys = new List<string>();
            string configKeys;
            foreach (var item in jo)
            {
                keys.Add(item.Key);
            }
            configKeys = string.Join(",", keys);
            var areaParams = _repository.GetAreaParams(model.areaid,model.type);
            if (areaParams != null)
            {
   
                if (configKeys.Equals(areaParams.configKeys))
                {
                    return;
                }
                var akeys = areaParams.configKeys.Split(',');
                keys.Concat(akeys).ToList().Distinct();
                areaParams.configKeys = string.Join(",", keys);
                _repository.UpdateAreaParams(areaParams);
            }
            else
            {
                _repository.Insert(new List<AreaParamsModel>() {
                 new AreaParamsModel()
                 {
                    areaid=model.areaid,
                    configKeys=configKeys,
                    type=model.type
                 }
                });
            }
        }
    }
}
