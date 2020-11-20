using Dapper;
using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                //近30天数据
                model.xAxis = xAxisTools.DataRange(30,true);
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
                var startTime = DateTime.Now.AddHours(-24);
                var endTime = DateTime.Now;
                var realTimeList = GetRealTimeList(startTime,endTime,type,value);
                data.xAxis = realTimeList;
                //获取缓存实时数据
                var list = _cache.GetAllList<UserActionModel>(areaid.ToString());
                //初始化
                List<int> all = new List<int>();
                List<int> android = new List<int>();
                List<int> ios = new List<int>();
                List<int> windows = new List<int>();
                for (int i = 0; i < realTimeList.Count; i++)
                {
                    DateTime st;
                    if (i==0)
                    {
                        st = Convert.ToDateTime(startTime.ToString($"yyyy-MM-dd {realTimeList[i]}"));
                    }
                    else
                    {
                        st = Convert.ToDateTime(startTime.ToString($"yyyy-MM-dd {realTimeList[i - 1]}"));
                    }
                    var et = Convert.ToDateTime(startTime.ToString($"yyyy-MM-dd {realTimeList[i]}"));
                    //时间段内数据
                    var dataItem = list.Where(i => i.date >= st && i.date < et);
                    //活跃用户
                    data.Active.All.Add(dataItem.Where(i => i.uid != 0).GroupBy(i => i.uid).Count());
                    data.Active.Android.Add(dataItem.Where(i => i.uid != 0&&i.platForm==   PlatFromEnum.Android.GetName()).GroupBy(i => i.uid).Count());
                    data.Active.IOS.Add(dataItem.Where(i => i.uid != 0&&i.platForm== PlatFromEnum.IOS.GetName()).GroupBy(i => i.uid).Count());
                    data.Active.Windows.Add(dataItem.Where(i => i.uid != 0&&i.platForm== PlatFromEnum.Windows.GetName()).GroupBy(i => i.uid).Count());

                    //注册用户
                    data.Register.All.Add(dataItem.Where(i => i.uid == 0).Count());
                    data.Register.Android.Add(dataItem.Where(i => i.uid == 0&&i.platForm== PlatFromEnum.Android.GetName()).Count());
                    data.Register.IOS.Add(dataItem.Where(i => i.uid == 0 && i.platForm == PlatFromEnum.IOS.GetName()).Count());
                    data.Register.Windows.Add(dataItem.Where(i => i.uid == 0 && i.platForm == PlatFromEnum.Windows.GetName()).Count());
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
        /// 实时时间x轴坐标列表
        /// </summary>
        /// <param name="type">0秒,1分,2时</param>
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
        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public AreaParamsModel GetAreaParams(int areaid) 
        {
            try
            {
                var res = _repository.GetAreaParams(areaid);
                if (res == null)
                {
                    res = new AreaParamsModel();
                }
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaParams:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 单场景分析
        /// </summary>
        /// <param name="areaid">区域id</param>
        /// <param name="days">天数</param>
        /// <param name="platFrom">平台</param>
        /// <param name="otherParam">其它参数</param>
        /// <param name="dateRange">日期范围</param>
        /// <returns></returns>
        public SingleSceneModel GetSingleSceneData(int areaid,int days,string platFrom,string otherParam,string dateRange)
        {
            try
            {
                SingleSceneModel data = new SingleSceneModel();
                List<string> datelist = new List<string>();
                string contition = $" and platForm='{platFrom}' ";
                List<string> legendData = new List<string>();
                if (platFrom== PlatFromEnum.All.GetName())
                {
                    contition = "";
                    legendData.Add(PlatFromEnum.All.GetName());
                }
                if (!string.IsNullOrEmpty(otherParam))
                {
                    legendData.Add(otherParam);
                }
                //近*天
                if (days!=0)
                {
                    var start = DateTime.Now.AddDays(-days);
                    data.xAxis = xAxisTools.DataRange(days,true);
                    var end = DateTime.Now;
                    datelist = xAxisTools.DataRange(days);
                    contition += $"and date between '{start}' and '{end}' ";
                    //数据   缓存数据
                    var unitData = _repository.GetActionData(areaid, contition);
                    //活跃
                    List<int> actv = new List<int>();
                    //注册
                    List<int> regst = new List<int>();
                    for (int i = 0; i < datelist.Count-1; i++)
                    {
                        var x = datelist[i];
                        DateTime st = Convert.ToDateTime(x);
                        //data.xAxis.Add(x);
                        var et = Convert.ToDateTime(datelist[i + 1]);
                        var acount = unitData.Where(i => i.uid!=0&& i.date>=st&&i.date<et).GroupBy(i=>i.uid).Count();
                        var rcount = unitData.Where(i => i.uid==0&& i.date>=st&&i.date<et).Count();
                        actv.Add(acount);
                        regst.Add(rcount);
                    }
                    data.ActiveData.Add(actv);
                    data.RegisterData.Add(regst);
                    data.legendData.Add(platFrom);
                }
                //日期范围
                if (!string.IsNullOrEmpty(dateRange))
                {
                    var daterange=dateRange.Split('至');
                    var start = Convert.ToDateTime(daterange[0].Trim());
                    var end = Convert.ToDateTime(daterange[1].Trim());
                    contition += $"and date between '{start}' and '{end}' ";
                    //data.xAxis = xAxisTools.DataRange(start,end);
                    //数据
                    var unitData = _repository.GetActionData(areaid, contition);
                    datelist = xAxisTools.DataRange(start,end);
                    data.xAxis = xAxisTools.DataRange(start, end, true);
                    List<int> actv = new List<int>();
                    //注册
                    List<int> regst = new List<int>();

                    for (int i = 0; i < datelist.Count-1; i++)
                    {
                        var x = datelist[i];
                        DateTime st = Convert.ToDateTime(x);
                        //data.xAxis.Add(x);
                        var et = Convert.ToDateTime(datelist[i + 1]);
                        var acount = unitData.Where(i => i.uid!=0&& i.date >= st && i.date < et).GroupBy(i=>i.uid).Count();
                        var rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et).Count();
                        actv.Add(acount);
                        regst.Add(rcount);
                    }
                    data.ActiveData.Add(actv);
                    data.RegisterData.Add(regst);
                    data.legendData.Add(platFrom);
                }
                if (!string.IsNullOrEmpty(otherParam))
                {
                    data.legendData.Add(otherParam);
                    //数据
                    var unitData = _repository.GetActionData(areaid, contition);
                    List<int> actv = new List<int>();
                    //注册
                    List<int> regst = new List<int>();

                    for (int i = 0; i < datelist.Count-1; i++)
                    {
                        var x = datelist[i];
                        DateTime st = Convert.ToDateTime(x);
                        //data.xAxis.Add(x);
                        var et = Convert.ToDateTime(datelist[i + 1]);
                        var acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et && i.data.Contains(otherParam)).GroupBy(i => i.uid).Count();
                        var rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et && i.data.Contains(otherParam)).Count();
                        actv.Add(acount);
                        regst.Add(rcount);
                    }
                    data.ActiveData.Add(actv);
                    data.RegisterData.Add(regst);
                    //data.legendData.Add(platFrom);
                }
            
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError($"SingleScene:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 用户画像
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="strat"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public UserPicModel GetUserPic(int areaid,DateTime start, DateTime end)
        {
            try
            {
                UserPicModel res = new UserPicModel();
                string contition = $" and date between '{start}' and '{end}'";
                //数据
                var unitData = _repository.GetActionData(areaid, contition);
                Dictionary<int, string> plats=new Dictionary<int, string>();
                PlatFromEnumExt.GetEnumAllNameAndValue<PlatFromEnum>(ref plats);
                foreach (var item in plats)
                {
                    string plat = item.Value;
                    if (plat != "All")
                    {
                        res.legendData.Add(plat);
                        seriesData aseries = new seriesData
                        {
                            name = plat,
                            value = unitData.Where(i => i.platForm == plat && i.uid != 0).GroupBy(i=>i.uid).Count()
                        };
                        seriesData rseries = new seriesData
                        {
                            name = plat,
                            value = unitData.Where(i => i.platForm == plat && i.uid == 0).Count()
                        };
                        res.ActiveData.Add(aseries);
                        res.RegisterData.Add(rseries);
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserPic:{e.Message}");
                throw;
            }

        }
        /// <summary>
        /// 漏斗图数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="platForm"></param>
        /// <param name="day"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="other"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public FunnelDataModel GetFunnelData(int areaid,string platForm,int days,DateTime? start,DateTime? end,string other,string otherValue="")
        {
            try
            {
                string contition = " ";
                if (platForm!=PlatFromEnum.All.GetName())
                {
                    contition += $" and platForm='{platForm}' ";
                }
                FunnelDataModel model = new FunnelDataModel();
                if (!string.IsNullOrEmpty(otherValue))
                {
                    model.lengdData = otherValue.Split(',').ToList();
                }
               
                if (days!=0)
                {
                    var st = DateTime.Now.AddDays(-days);
                    var et = DateTime.Now;
                    contition += $"and date between '{st}' and '{et}' ";
                }
                else
                {
                    contition += $"and date between '{start}' and '{end}' ";
                }
                //数据
                var unitData = _repository.GetActionData(areaid, contition);
                //List<string> dataValue = otherValue.Split(',').ToList();
                List<dataItem> aitem = new List<dataItem>();
                List<dataItem> ritem = new List<dataItem>();
                double asum = 0;
                double rsum = 0;
                for (int x = 0; x < model.lengdData.Count; x++)
                {
                    if (x==0)
                    {
                        asum = unitData.Where(i => i.uid != 0).Where(i =>
                        {
                            JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
                            bool res = jo.Value<string>(other) == model.lengdData[x] ? true : false;
                            return res;
                        }).GroupBy(i => i.uid).ToList().Count;
                        rsum = unitData.Where(i => i.uid == 0).Where(i =>
                        {
                            JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
                            bool res = jo.Value<string>(other) == model.lengdData[x] ? true : false;
                            return res;
                        }).ToList().Count;
                        aitem.Add(new dataItem() { value = 100, name = model.lengdData[x] });
                        ritem.Add(new dataItem() { value = 100, name = model.lengdData[x] });
                    }
                    else
                    {
                        if (asum==0)
                        {
                            asum = 1;
                        }
                        if (rsum==0)
                        {
                            rsum = 1;
                        }
                        var acount = unitData.Where(i => i.uid != 0).Where(i =>
                        {
                            JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
                            bool res = jo.Value<string>(other) == model.lengdData[x] ? true : false;
                            return res;
                        }).GroupBy(i => i.uid).ToList().Count;
                        //model.activeData.Add(acount);
                        aitem.Add(new dataItem() { value = (int)((acount / asum)*100), name = model.lengdData[x] });
                        var rcount = unitData.Where(i => i.uid == 0).Where(i =>
                        {
                            JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
                            bool res = jo.Value<string>(other) == model.lengdData[x] ? true : false;
                            return res;
                        }).ToList().Count;

                        ritem.Add(new dataItem() { value = (int)((rcount/rsum)*100), name = model.lengdData[x] });
                    }
                }

                model.activeData = aitem;
                model.registerData = ritem;
                return model;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetFunnelData:{e.Message}");
                throw;
            }
        }
    }
}
