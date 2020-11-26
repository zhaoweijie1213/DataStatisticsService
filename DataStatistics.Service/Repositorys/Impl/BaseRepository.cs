using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class BaseRepository : IBaseRepository
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string ConnectionString { get; set; }
        /// <summary>
        /// 创建实例
        /// </summary>
        private IDbConnection Db
        {
            get
            {
                //var db = CreateConnection(ConnectionString);
                IDbConnection db = new MySqlConnection(ConnectionString);
                return db;
            }
        }
        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbConnection _db
        {
            get { return Db; }
        }
        /// <summary>
        /// dbconnection
        /// </summary>
        /// <param name = "connectionString" > 连接字符串 </ param >
        /// < returns ></ returns >
        //public IDbConnection CreateConnection(string connectionString)
        //{
        //    try
        //    {
        //        return new DbContext().ConnectionString(connectionString ?? ConnectionString,
        //           new MySqlProvider()).CommandTimeout(1000);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }
}
