using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Services.DataProcessing;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.DataProcessingl.Impl
{
    public class DataProcessing: ICapSubscribe, IDataProcessing
    {
        private readonly ICacheManage _cache;
        public DataProcessing(ICacheManage cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        [CapSubscribe("Data.Recive")]
        public void SubscribeWithnoController(UserActionModel model)
        {
            //向list添加元素
            long length = _cache._redisProvider.RPushX<UserActionModel>(model.areaid.ToString(), model);
            
        }
    }
}
