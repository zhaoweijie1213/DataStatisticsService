using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Model.ViewModel
{
    public class FunnelDataModel
    {
        /// <summary>
        /// 图例数据
        /// </summary>
        public List<string> lengdData { get; set; } = new List<string>();
        /// <summary>
        /// 活跃
        /// </summary>
        public List<dataItem> activeData { get; set; } = new List<dataItem>();
        /// <summary>
        /// 注册
        /// </summary>
        public List<dataItem> registerData { get; set; } = new List<dataItem>();

    }
    public class dataItem
    {
        public int value { get; set; }
        public string name { get; set; }
    }
}
