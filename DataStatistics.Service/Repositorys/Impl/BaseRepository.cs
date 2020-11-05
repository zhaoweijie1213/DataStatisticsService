using FluentData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class BaseRepository : IBaseRepository
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string ConnectionString { get; set; }
        public BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        private IDbContext Db
        {
            get
            {
                var db = CreateConnection(ConnectionString);

                return db;
            }
        }
        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext _db
        {
            get { return Db; }
        }
        /// <summary>
        /// dbconnection
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public IDbContext CreateConnection(string connectionString)
        {
            try
            {
                return new DbContext().ConnectionString(connectionString ?? ConnectionString,
                   new MySqlProvider ()).CommandTimeout(1000);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
