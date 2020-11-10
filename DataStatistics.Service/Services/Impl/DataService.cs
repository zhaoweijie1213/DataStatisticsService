using Dapper;
using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Repositorys;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Services.Impl
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IMJLogOtherRepository _repository;
        private readonly ICacheManage _cache;
        public DataService(IMJLogOtherRepository repository, ILogger<DataService> logger, ICacheManage cache)
        {
            _logger = logger;
            _repository = repository;
            _cache = cache;
        }
        /// <summary>
        /// 数据库获取数据
        /// </summary>
        /// <returns></returns>
        public List<UserActionModel> GetUserActions()
        {
            try
            {
                var res = _repository.GetUserActions();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserActions:{e.Message}");
                throw;
            }
        }

        ///// <summary>
        ///// 昨日概况
        ///// </summary>
        ///// <returns></returns>
        public List<OverallSituationModel> DataSituationForYestoday(int areaid)
        {
            try
            {
                var res = _repository.GetSituation(areaid);
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"DataSituationForYestoday:{e.Message}");
                throw;
            } 
        }

        /// <summary>
        /// 近期趋势
        /// </summary>
        /// <returns></returns>
        public DaysDataModel ThirtyDaysData(int areaid,DateTime time)
        {
            DaysDataModel model = new DaysDataModel();

            try
            {
                var res = _repository.GetThirtyDaysData(areaid,time).OrderBy(i=>i.dataTime).ToList();
                List<int> all = new List<int>();
                List<int> android = new List<int>();
                List<int> ios = new List<int>();
                List<int> windows = new List<int>();
                for (int i=0;i<30;i++)
                {
                    var itemDate= time.AddDays(i);
                    var itemData = res.Where(i=>i.dataTime==itemDate).ToList();
                    if (itemData.Count>0)
                    {
                        model.Active.All.Add(itemData.FirstOrDefault(i => i.platForm == "All").activeUsers);
                        model.Register.All.Add(itemData.FirstOrDefault(i => i.platForm == "All").registeredUsers);
                        model.Active.Android.Add(itemData.FirstOrDefault(i => i.platForm == "Android").activeUsers);
                        model.Register.Android.Add(itemData.FirstOrDefault(i => i.platForm == "Android").registeredUsers);
                        model.Active.IOS.Add(itemData.FirstOrDefault(i => i.platForm == "IOS").activeUsers);
                        model.Register.IOS.Add(itemData.FirstOrDefault(i => i.platForm == "IOS").registeredUsers);
                        model.Active.Windows.Add(itemData.FirstOrDefault(i => i.platForm == "Windows").activeUsers);
                        model.Register.Windows.Add(itemData.FirstOrDefault(i => i.platForm == "Windows").registeredUsers);
                    }
                    else
                    {
                        model.Active.All.Add(0);
                        model.Register.All.Add(0);
                        model.Active.Android.Add(0);
                        model.Register.Android.Add(0);
                        model.Active.IOS.Add(0);
                        model.Register.IOS.Add(0);
                        model.Active.Windows.Add(0);
                        model.Register.Windows.Add(0);
                    }
                }
                return model;
            }
            catch (Exception e)
            {
                _logger.LogError($"ThirtyDaysData:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type">0秒,1分,2时</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DaysDataModel RealTimeData(int areaid,int type,int value)
        {
            DaysDataModel data = new DaysDataModel();
            try
            {
                var startTime = DateTime.Now.Date;
                var endTime = DateTime.Now;
                var realTimeList = GetRealTimeList(startTime,endTime,type,value);
                #region 向redis添加测试数据
                //var todayData = _repository._db.Query<UserActionModel>($"select * from log_userAction where date between '{startTime}' and '{endTime}' ").ToList();
                //foreach (var item in todayData)
                //{
                //    //var s = _cache._redisProvider.SMembers<UserActionModel>(item.areaid.ToString());
                //    //向list添加元素
                //    _cache._redisProvider.RPushX<UserActionModel>(item.areaid.ToString(),item);
                //}
                #endregion
                //获取缓存实时数据
                var list = _cache.GetAllList<UserActionModel>("800");
                //初始化
                List<int> all = new List<int>();
                List<int> android = new List<int>();
                List<int> ios = new List<int>();
                List<int> windows = new List<int>();
                for (int i = 0; i < realTimeList.Count; i++)
                {
                    var st = Convert.ToDateTime(startTime.ToString($"yyyy-MM-dd {realTimeList[i]}"));
                    var et = Convert.ToDateTime(startTime.ToString($"yyyy-MM-dd {realTimeList[i]}"));
                    //时间段内数据
                    var dataItem = list.Where(i => i.date >= st && i.date < et);
                    //活跃用户
                    data.Active.All.Add(dataItem.Where(i => i.uid != 0).GroupBy(i => i.uid).Count());
                    data.Active.Android.Add(dataItem.Where(i => i.uid != 0&&i.platForm==   PlatFromEnum.Android.GetName()).GroupBy(i => i.uid).Count());
                    data.Active.IOS.Add(dataItem.Where(i => i.uid != 0&&i.platForm== "IOS").GroupBy(i => i.uid).Count());
                    data.Active.Windows.Add(dataItem.Where(i => i.uid != 0&&i.platForm== "Windows").GroupBy(i => i.uid).Count());

                    //注册用户
                    data.Register.All.Add(dataItem.Where(i => i.uid == 0).Count());
                    data.Register.Android.Add(dataItem.Where(i => i.uid == 0&&i.platForm== "Android").Count());
                    data.Register.IOS.Add(dataItem.Where(i => i.uid == 0 && i.platForm == "Android").Count());
                    data.Register.Windows.Add(dataItem.Where(i => i.uid == 0 && i.platForm == "Android").Count());
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RealTimeData:{e.Message}");
                throw;
            }
           
            return data;
        }

        /// <summary>
        /// 实时时间列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<string> GetRealTimeList(DateTime startTime,DateTime endTime,int type, int value)
        {
            try
            {
                TimeSpan time = endTime - startTime;
                List<string> list = new List<string>();
                if (type == 1)
                {
                    var s = time.TotalMinutes;
                    int count = (int)s / value;
                    for (int i = 0; i <= count; i++)
                    {
                        var xValue = startTime.AddMinutes(i * value);
                        list.Add(xValue.ToString("HH:mm"));
                    }
                    if (s % value > 0)
                    {
                        list.Add(endTime.ToString("HH:mm"));
                    }
                    if (s % value < 0)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.Add(endTime.ToString("HH:mm"));
                    }
                }
                if (type == 2)
                {
                    var s = time.TotalHours;
                    int count = (int)s / value;
                    for (int i = 0; i <= count; i++)
                    {
                        var xValue = startTime.AddHours(i * value);
                        list.Add(xValue.ToString("HH:mm"));
                    }
                    if (s % value > 0)
                    {
                        list.Add(endTime.ToString("HH:mm"));
                    }
                    if (s % value < 0)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.Add(endTime.ToString("HH:mm"));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRealTimeList{e.Message}");
                throw;
            }
        }
    }
}
