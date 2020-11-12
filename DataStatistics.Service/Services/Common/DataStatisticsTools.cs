using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using DataStatistics.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStatistics.Service.Services.Common
{
    public static class DataStatisticsTools
    {
        /// <summary>
        /// 按时间分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="xAxis"></param>
        /// <param name="start"></param>
        /// <param name="userState"></param>
        /// <param name="timeType">0：日期,1：时间</param>
        /// <returns></returns>
        public static List<int> GroupByTimeRange(this List<UserActionModel> list,List<string> xAxis, DateTime start, UserState userState, int timeType=0)
        {
            List<int> data = new List<int>();
            if (timeType==0)
            {
                for (int i = 0; i < xAxis.Count; i++)
                {
                    DateTime st;
                    if (i == 0)
                    {
                        st = Convert.ToDateTime(start.ToString($"yyyy-{start}"));
                    }
                    else
                    {
                        st = Convert.ToDateTime(start.ToString($"yyyy-{xAxis[i - 1]}"));
                    }
                    var et = Convert.ToDateTime(start.ToString($"yyyy-{xAxis[i]}"));
                    int unit = 0;
                    if (userState==UserState.Active)
                    {
                        unit = list.Where(i => i.date >= st && i.date < et).GroupBy(i => i.uid).Count();
                    }
                    if (userState == UserState.Register)
                    {
                        unit = list.Where(i => i.date >= st && i.date < et).Count();
                    }
                    data.Add(unit);
                }
            }
            return data;
        }
    }
}
