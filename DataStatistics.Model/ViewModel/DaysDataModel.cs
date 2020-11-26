using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStatistics.Model.ViewModel
{
    public class DaysDataModel
    {
        /// <summary>
        /// X轴坐标
        /// </summary>
        public List<string> xAxis { get; set; }
        /// <summary>
        /// 活跃
        /// </summary>
        public platForm Active { get; set; } = new platForm();
        /// <summary>
        /// 注册
        /// </summary>
        public platForm Register { get; set; } = new platForm();
    }
    /// <summary>
    /// 平台数据
    /// </summary>
    public class platForm 
    {
        public List<int> All { get; set; } = new List<int>();
        public List<int> Android { get; set; } = new List<int>();
        public List<int> IOS { get; set; } = new List<int>();
        public List<int> Windows { get; set; } = new List<int>();
    }
    /// <summary>
    /// 实时数据处理
    /// </summary>
    [Serializable]
    public class JobRealData
    {
        /// <summary>
        /// 活跃
        /// </summary>
        public plat Active { get; set; }
        /// <summary>
        /// 注册
        /// </summary>
        public plat Register { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime dateTime { get; set; }
    }

    /// <summary>
    /// 平台
    /// </summary>
    [Serializable]
    public class plat 
    {
        public int All { get; set; }
        public int Android { get; set; }
        public int IOS { get; set; }
        public int Windows { get; set; }
    }
}
