using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Repositorys;
using Microsoft.Extensions.Logging;
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
        public ThirtyDaysDataModel ThirtyDaysData(int areaid,DateTime time)
        {
            ThirtyDaysDataModel model = new ThirtyDaysDataModel();

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
                _logger.LogError($"DataSituationForYestoday:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <returns></returns>
        public string ActiveData()
        {
            return "";
        }


    }
}
