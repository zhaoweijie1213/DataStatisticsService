﻿using System;
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
        public List<string> xAxis { get; set; }
        /// <summary>
        /// 图例数据
        /// </summary>
        public List<string> legendData { get; set; }
        /// <summary>
        /// 活跃数据
        /// </summary>
        public List<List<string>> ActiveData { get; set; }
        /// <summary>
        /// 注册数据
        /// </summary>
        public List<List<string>> RegisterData { get; set; }
    }
}
