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

        /// <summary>
        /// 昨日概况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<OverallSituationModel> DataSituationForYestoday(int areaid, int type)
        {
            try
            {
                List<OverallSituationModel> res = new List<OverallSituationModel>();
                res = _repository.GetSituation(areaid, type);
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
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public DaysDataModel ThirtyDaysData(int areaid, int type, DateTime time)
        {
            DaysDataModel model = new DaysDataModel();

            try
            {
                var res = _repository.GetThirtyDaysData(areaid,type,time).OrderBy(i=>i.dataTime).ToList();
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
                        model.Active.All.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.All.GetName()).activeUsers);
                        model.Register.All.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.All.GetName()).registeredUsers);
                        model.Active.Android.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.Android.GetName()).activeUsers);
                        model.Register.Android.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.Android.GetName()).registeredUsers);
                        model.Active.IOS.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.IOS.GetName()).activeUsers);
                        model.Register.IOS.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.IOS.GetName()).registeredUsers);
                        model.Active.Windows.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.Windows.GetName()).activeUsers);
                        model.Register.Windows.Add(itemData.FirstOrDefault(i => i.platForm == PlatFromEnum.Windows.GetName()).registeredUsers);
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
        /// 获取版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public List<string> GetVersion(int areaid)
        {
            try
            {
                var res = _repository.GetAreaVersion(areaid).Select(i => i.version).OrderByDescending(i=>i).ToList();
                //var res = _cache._redisProvider.LRange<string>($"v_{areaid}", 0, length).OrderByDescending(i=>i).ToList();
                return res; 
            }
            catch (Exception e)
            {
                _logger.LogError($"GetVersion:{e.Message}");
                return null;
            }
        }
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public DaysDataModel RealTimeData(int areaid,int value, int type,string version)
        {
            DaysDataModel data = new DaysDataModel();
            try
            {
                string ridesKey = "";
                if (string.IsNullOrEmpty(version))
                {
                    ridesKey = $"r_{value}_{type}_{areaid}";
                }
                else
                {
                    ridesKey = $"r_{value}_{type}_{version}_{areaid}";
                }
                long length = _cache._redisProvider.LLen(ridesKey);
                List<JobRealData> list = _cache._redisProvider.LRange<JobRealData>(ridesKey,0,length);
                var startTime = DateTime.Now.AddHours(-24);
                var endTime = DateTime.Now;
                list = list.Where(i => i.dateTime >= startTime && i.dateTime <= endTime).OrderBy(i=>i.dateTime).ToList();
                data.xAxis = list.Select(i=>i.dateTime.ToString("HH:mm")).ToList();
                data.Active = new platForm()
                {
                    All = list.Select(i=>i.Active.All).ToList(),
                    Android= list.Select(i => i.Active.Android).ToList(),
                    IOS= list.Select(i => i.Active.IOS).ToList(),
                    Windows= list.Select(i => i.Active.Windows).ToList()
                };
                data.Register = new platForm()
                {
                    All = list.Select(i => i.Register.All).ToList(),
                    Android = list.Select(i => i.Register.Android).ToList(),
                    IOS = list.Select(i => i.Register.IOS).ToList(),
                    Windows = list.Select(i => i.Register.Windows).ToList()
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"RealTimeData:{e.Message}");
                throw;
            }
           
            return data;
        }





        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public AreaParamsModel GetAreaParams(int areaid,int type) 
        {
            try
            {
                var res = _repository.GetAreaParams(areaid,type);
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
        public SingleSceneModel GetSingleSceneData(int areaid,int days,string platFrom,string otherParam,string dateRange,int type,string version)
        {
            try
            {
                SingleSceneModel data = new SingleSceneModel();
                List<string> datelist = new List<string>();
                string contition = $"  and type={type} ";
                if (!string.IsNullOrEmpty(version))
                {
                    contition += $"  and version='{version}' ";
                }
                if (platFrom != PlatFromEnum.All.GetName())
                {
                    contition = $" and platForm='{platFrom}' ";
                }
                else
                {
                    contition = $" and platForm in('IOS','Android','Windows') ";
                }
                List<string> legendData = new List<string>();
                legendData.Add(platFrom);
                if (!string.IsNullOrEmpty(otherParam))
                {
                    legendData.Add(otherParam);
                }
                data.legendData = legendData;
                //近*天
                if (days!=0)
                {
                    var start = DateTime.Now.Date.AddDays(-days);
                    data.xAxis = xAxisTools.DataRange(days,true);
                    var end = DateTime.Now;
                    datelist = xAxisTools.DataRange(days);
                    //contition += $"and date between '{start}' and '{end}' ";
                    //数据   缓存数据
                    //var unitData = _repository.GetActionData(areaid, contition);
                    List<UserActionModel> unitData = new List<UserActionModel>();
                    if (string.IsNullOrEmpty(version))
                    {
                        unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).Where(i => i.type == type && i.date >= start && i.date <= end).ToList();
                    }
                    else
                    {
                        unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).Where(i => i.type == type && i.version == version && i.date >= start && i.date <= end).ToList();
                    }
                    //活跃
                    List<int> actv = new List<int>();
                    //注册
                    List<int> regst = new List<int>();
                    for (int i = 0; i < datelist.Count-1; i++)
                    {
                        var x = datelist[i];
                        DateTime st = Convert.ToDateTime(x);
                        var et = Convert.ToDateTime(datelist[i + 1]);
                        int acount = 0;
                        int rcount = 0;
                        if (platFrom == PlatFromEnum.All.GetName())
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et).Count();
                        }
                        else
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).Count();
                        }
                        actv.Add(acount);
                        regst.Add(rcount);
                    }
                    data.ActiveData.Add(actv);
                    data.RegisterData.Add(regst);
                    //data.legendData.Add(platFrom);
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
                    //var unitData = _repository.GetActionData(areaid, contition);//从缓存取30天数据
                    List<UserActionModel> unitData = new List<UserActionModel>();
                    if (string.IsNullOrEmpty(version))
                    {
                        unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).Where(i => i.type == type && i.date >= start && i.date <= end).ToList();
                    }
                    else
                    {
                        unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).Where(i => i.type == type && i.version == version && i.date >= start && i.date <= end).ToList();
                    }
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
                        int acount = 0;
                        int rcount = 0;
                        if (platFrom == PlatFromEnum.All.GetName())
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et).Count();
                        }
                        else
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).Count();
                        }
                        actv.Add(acount);
                        regst.Add(rcount);
                    }
                    data.ActiveData.Add(actv);
                    data.RegisterData.Add(regst);
                    //data.legendData.Add(platFrom);
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
                        int acount = 0;
                        int rcount = 0;
                        if (platFrom == PlatFromEnum.All.GetName())
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et).Count();
                        }
                        else
                        {
                            acount = unitData.Where(i => i.uid != 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).GroupBy(i => i.uid).Count();
                            rcount = unitData.Where(i => i.uid == 0 && i.date >= st && i.date < et && i.platForm.ToLower() == platFrom.ToLower()).Count();
                        }
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
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public UserPicModel GetUserPic(int areaid,DateTime start, DateTime end,int type,string version)
        {
            try
            {
                UserPicModel res = new UserPicModel();
                string contition = $" and type={type} and version='{version}' and date between '{start}' and '{end}'";
                if (string.IsNullOrEmpty(version))
                {
                    contition = $" and type={type}  and date between '{start}' and '{end}'";
                }
                //数据
                //var unitData = _repository.GetActionData(areaid, contition);//从缓存取30天数据
                List<UserActionModel> unitData = new List<UserActionModel>();
                if (string.IsNullOrEmpty(version))
                {
                    unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).ToList().Where(i => i.type == type && i.date >= start && i.date <= end).ToList();
                }
                else
                {
                    unitData = _cache.GetRawDataForThirty(areaid.ToString(),start,end).ToList().Where(i => i.type == type && i.version == version && i.date >= start && i.date <= end).ToList();
                }
                Dictionary<int, string> plats=new Dictionary<int, string>();
                plats = PlatFromEnumExt.GetEnumAllNameAndValue<PlatFromEnum>();
                foreach (var item in plats)
                {
                    string plat = item.Value;
                    if (plat != "All")
                    {
                        res.legendData.Add(plat);
                        seriesData aseries = new seriesData
                        {
                            name = plat,
                            value = unitData.Where(i => i.platForm?.ToLower() == plat.ToLower() && i.uid != 0).GroupBy(i=>i.uid).Count()
                        };
                        seriesData rseries = new seriesData
                        {
                            name = plat,
                            value = unitData.Where(i => i.platForm?.ToLower() == plat.ToLower() && i.uid == 0).Count()
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
        //public FunnelDataModel GetFunnelData(int areaid, string platForm, int days, DateTime? start, DateTime? end, string other, string otherValue, int type, string version)
        //{
        //    try
        //    {
        //        string contition = $"  and type={type} ";
        //        if (!string.IsNullOrEmpty(version))
        //        {
        //            contition += $" and version='{version}' ";
        //        }
        //        if (platForm!=PlatFromEnum.All.GetName())
        //        {
        //            contition += $" and platForm='{platForm}' ";
        //        }
        //        else
        //        {
        //            contition = $" and platForm in('IOS', 'Android', 'Windows') ";
        //        }
        //        FunnelDataModel model = new FunnelDataModel();
        //        if (!string.IsNullOrEmpty(otherValue))
        //        {
        //            model.lengdData = otherValue.Split(',').ToList();
        //        }
        //        if (days!=0)
        //        {
        //            var st = DateTime.Now.AddDays(-days);
        //            var et = DateTime.Now;
        //            contition += $"and date between '{st}' and '{et}' ";
        //        }
        //        else
        //        {
        //            contition += $"and date between '{start}' and '{end}' ";
        //        }
        //        //数据
        //        var unitData = _repository.GetActionData(areaid, contition);
        //        List<dataItem> aitem = new List<dataItem>();
        //        List<dataItem> ritem = new List<dataItem>();
        //        double asum = 0;
        //        double rsum = 0;
        //        for (int x = 0; x < model.lengdData.Count; x++)
        //        {
        //            if (x==0)
        //            {
        //                asum = unitData.Where(i => i.uid != 0).Where(i =>
        //                {
        //                    JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
        //                    bool res = jo.Value<string>(other) == model.lengdData[x];
        //                    return res;
        //                }).GroupBy(i => i.uid).ToList().Count;
        //                rsum = unitData.Where(i => i.uid == 0).Where(i =>
        //                {
        //                    JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
        //                    bool res = jo.Value<string>(other) == model.lengdData[x];
        //                    return res;
        //                }).ToList().Count;
        //                aitem.Add(new dataItem() { value = 100, name = model.lengdData[x] });
        //                ritem.Add(new dataItem() { value = 100, name = model.lengdData[x] });
        //            }
        //            else
        //            {
        //                if (asum==0)
        //                {
        //                    asum = 1;
        //                }
        //                if (rsum==0)
        //                {
        //                    rsum = 1;
        //                }
        //                var acount = unitData.Where(i => i.uid != 0).Where(i =>
        //                {
        //                    JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
        //                    bool res = jo.Value<string>(other) == model.lengdData[x];
        //                    return res;
        //                }).GroupBy(i => i.uid).ToList().Count;
        //                //model.activeData.Add(acount);
        //                aitem.Add(new dataItem() { value = (int)((acount / asum)*100), name = model.lengdData[x] });
        //                var rcount = unitData.Where(i => i.uid == 0).Where(i =>
        //                {
        //                    JObject jo = JsonConvert.DeserializeObject<JObject>(i.data);
        //                    bool res = jo.Value<string>(other) == model.lengdData[x];
        //                    return res;
        //                }).ToList().Count;

        //                ritem.Add(new dataItem() { value = (int)((rcount/rsum)*100), name = model.lengdData[x] });
        //            }
        //        }

        //        model.activeData = aitem;
        //        model.registerData = ritem;
        //        return model;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"GetFunnelData:{e.Message}");
        //        throw;
        //    }
        //}
    }
}
