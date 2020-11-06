using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStatistics.Model.mj_log_other
{
    /// <summary>
    /// 总体情况
    /// </summary>
    [Table("log_overall_situation")]
    public class OverallSituationModel
    {
        public OverallSituationModel()
        {
            activeUsers = 0;
            registeredUsers = 0;
        }
        /// <summary>
        /// 编号
        /// </summary>
        [NotMapped]
        public int id { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        public int areaid { get; set; }
        /// <summary>
        /// 活跃用户
        /// </summary>
        public int activeUsers { get; set; }
        /// <summary>
        /// 注册用户
        /// </summary>
        public int registeredUsers { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string platForm { get; set; }
        /// <summary>
        /// 数据时间
        /// </summary>
        public DateTime dataTime { get; set; }
    }
}
