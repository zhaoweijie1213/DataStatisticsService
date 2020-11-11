using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStatistics.Model.mj_log_other
{
    [Table("log_area_param")]
    public class AreaParamsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        public int areaid { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string configKeys { get; set; }
    }
}
