using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Model.ViewModel
{
    public class UserPicModel
    {
        /// <summary>
        /// 图例
        /// </summary>
        public List<string> legendData { get; set; } = new List<string>();
        /// <summary>
        /// 数据
        /// </summary>
        public List<seriesData> ActiveData { get; set; } = new List<seriesData>();
        /// <summary>
        /// 数据
        /// </summary>
        public List<seriesData> RegisterData { get; set; } = new List<seriesData>();
    }
    public class seriesData
    { 
        public int value { get; set; }
        public string name { get; set; }
    }
}
