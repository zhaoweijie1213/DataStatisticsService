using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStatistics.Model.mj_log_other
{
    [Table("log_area_version")]
    public class AreaVersion
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 大厅
        /// </summary>
        public int areaid { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
    }
}
