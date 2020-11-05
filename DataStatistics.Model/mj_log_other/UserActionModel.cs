using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Model.mj_log_other
{
    [Serializable]
    public class UserActionModel
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public int id { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public int type { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime date { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public int areaid { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public int uid { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public string version { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>

        public string data { get; set; }


        /// <summary>
        /// Desc:包渠道
        /// Default:
        /// Nullable:False
        /// </summary>

        public string packageChannel { get; set; }


        /// <summary>
        /// Desc:平台
        /// Default:
        /// Nullable:False
        /// </summary>

        public string platForm { get; set; }


        /// <summary>
        /// Desc:设备
        /// Default:
        /// Nullable:False
        /// </summary>

        public string device { get; set; }

    }
}
