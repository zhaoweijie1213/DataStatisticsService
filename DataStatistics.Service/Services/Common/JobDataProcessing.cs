using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Services.Common
{
    public static class JobDataProcessing
    {
        /// <summary>
        /// 时间粒度数据处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static List<JobRealData> GetDataList(List<UserActionModel> data, DateTime time)
        {
            List<JobRealData> reg = new List<JobRealData>() {
                                new JobRealData()
                                {
                                    //活跃用户
                                    Active=new plat()
                                    {
                                         All=data.Where(i=>i.uid!=0).GroupBy(i=>i.uid).Count(),
                                         Android=data.Where(i=>i.uid!=0&&i.platForm.ToLower()==PlatFromEnum.Android.GetName().ToLower()).GroupBy(i=>i.uid).Count(),
                                         IOS=data.Where(i=>i.uid!=0&&i.platForm.ToLower()==PlatFromEnum.IOS.GetName().ToLower()).GroupBy(i=>i.uid).Count(),
                                         Windows=data.Where(i=>i.uid!=0&&i.platForm.ToLower()==PlatFromEnum.Windows.GetName().ToLower()).GroupBy(i=>i.uid).Count()
                                    },
                                     //注册用户
                                    Register=new plat()
                                    {
                                        All=data.Where(i=>i.uid==0).Count(),
                                        Android=data.Where(i=>i.uid==0&&i.platForm.ToLower()==PlatFromEnum.Android.GetName().ToLower()).Count(),
                                        IOS=data.Where(i=>i.uid==0&&i.platForm.ToLower()==PlatFromEnum.IOS.GetName().ToLower()).Count(),
                                        Windows=data.Where(i=>i.uid==0&&i.platForm.ToLower()==PlatFromEnum.Windows.GetName().ToLower()).Count(),
                                    },
                                    dateTime=time
                                }
                            };
            return reg;
        }
    }
}
