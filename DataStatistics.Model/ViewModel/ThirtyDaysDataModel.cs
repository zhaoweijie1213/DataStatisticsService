using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStatistics.Model.ViewModel
{
    public class ThirtyDaysDataModel
    {
        /// <summary>
        /// 活跃
        /// </summary>
        public platForm Active { get; set; } = new platForm();
        /// <summary>
        /// 注册
        /// </summary>
        public platForm Register { get; set; } = new platForm();
    }
    public class platForm 
    {
        public List<int> All { get; set; } = new List<int>();
        public List<int> Android { get; set; } = new List<int>();
        public List<int> IOS { get; set; } = new List<int>();
        public List<int> Windows { get; set; } = new List<int>();
    }
}
