using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataStatistics.Service.Repositorys
{
    public interface IBaseRepository
    {
        /// <summary>
        /// 数据库实例
        /// </summary>
        IDbConnection _db { get; }
        ///// <summary>
        ///// dbconnection
        ///// </summary>
        ///// <param name="connectionString">连接字符串</param>
        ///// <returns></returns>
        //IDbContext CreateConnection(string connectionString);
    }
}
