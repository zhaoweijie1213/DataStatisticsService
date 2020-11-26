using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DataStatistics.Api.Enums
{
    public enum ResultCode : int
    {
        [Description("操作成功")]
        Success = 0,
        [Description("参数错误")]
        ErrorParams = 2,
        [Description("操作失败")]
        Error = -1,
    }
}
