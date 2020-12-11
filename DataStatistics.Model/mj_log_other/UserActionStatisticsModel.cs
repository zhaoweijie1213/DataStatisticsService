using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStatistics.Model.mj_log_other
{
    [Table("log_userActionStatistics")]
    public class UserActionStatisticsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 大厅id
        /// </summary>
        public int areaid { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string platForm { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string uuid { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int uid { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime added { get; set; }
    }
}
