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
        /// 数据
        /// </summary>
        public List<string> seriesData { get; set; } = new List<string>();

    }
}
