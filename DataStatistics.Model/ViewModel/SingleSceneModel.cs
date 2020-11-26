using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Model.ViewModel
{
    /// <summary>
    /// 单场景
    /// </summary>
    public class SingleSceneModel
    {
        /// <summary>
        /// x坐标轴
        /// </summary>
        public List<string> xAxis { get; set; } = new List<string>();
        /// <summary>
        /// 图例数据
        /// </summary>
        public List<string> legendData { get; set; } = new List<string>();
        /// <summary>
        /// 活跃数据
        /// </summary>
        public List<List<int>> ActiveData { get; set; } = new List<List<int>>();
        /// <summary>
        /// 注册数据
        /// </summary>
        public List<List<int>> RegisterData { get; set; } = new List<List<int>>();
    }
}
