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
        /// <param name="hasYear">显示年份</param>
        /// <returns></returns>
        public static List<string> DataRange(int days,bool hasYear=false)
        {
            List<string> dateList = new List<string>();
            if (hasYear)
            {
               
                DateTime times = DateTime.Now.Date.AddDays(-days);
                for (int i = 0; i < days; i++)
                {
                    dateList.Add(times.AddDays(i).ToString("MM-dd"));
                }
            }
            else
            {
                DateTime times = DateTime.Now.Date.AddDays(-days);
                for (int i = 0; i <= days; i++)
                {
                    dateList.Add(times.AddDays(i).ToString("yyyy-MM-dd"));
                }
            }

            return dateList;
        }

        /// <summary>
        /// 日期范围
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="hasYear">显示年份</param>
        /// <returns></returns>
        public static List<string> DataRange(DateTime start,DateTime end,bool hasYear=false)
        {
            List<string> dateList = new List<string>();
            TimeSpan times = end - start;
            if (hasYear)
            {
                for (int i = 0; i < times.TotalDays; i++)
                {
                    dateList.Add(start.AddDays(i).ToString("MM-dd"));
                }
            }
            else
            {
                for (int i = 0; i <= times.TotalDays; i++)
                {
                    dateList.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
                }
            }
            return dateList;
        }
        /// <summary>
        /// 实时时间x轴坐标列表
        /// </summary>
        /// <param name="type">0秒,1分,2时</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> GetRealTimeList(DateTime startTime, DateTime endTime, int type, int value)
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
    }
}
