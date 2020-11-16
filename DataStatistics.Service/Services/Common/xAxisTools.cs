using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.Common
{
    public static class xAxisTools
    {
        /// <summary>
        /// 获取近几天数据
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static List<string> DataRange(int days)
        {
            List<string> dateList = new List<string>();
            DateTime times = DateTime.Now.Date.AddDays(-days);
            for (int i = 0; i <= days; i++)
            {
                dateList.Add(times.AddDays(i).ToString("yyyy-MM-dd"));
            }
            return dateList;
        }

        /// <summary>
        /// 日期范围
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<string> DataRange(DateTime start,DateTime end)
        {
            List<string> dateList = new List<string>();
            TimeSpan times = end - start;
            for (int i = 0; i <= times.TotalDays; i++)
            {
                dateList.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
            }
            return dateList;
        }
    }
}
