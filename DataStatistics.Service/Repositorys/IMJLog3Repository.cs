using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Repositorys
{
    public interface IMJLog3Repository
    {
        /// <summary>
        /// 获取大厅id
        /// </summary>
        /// <returns></returns>
        List<int> GetGameid();
    }
}
