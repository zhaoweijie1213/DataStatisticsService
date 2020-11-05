using DataStatistics.Model.mj_log_other;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services
{
    public interface ICacheManage
    {
        bool SetUserAction(List<UserActionModel> list);
        List<UserActionModel> GetUserAction();
    }
}
