﻿using Dapper;
using Dapper.Contrib.Extensions;
using DataStatistics.Model.mj_log_other;
using DataStatistics.Model.ViewModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataStatistics.Service.Repositorys.Impl
{
    public class MJLogOtherRepository : BaseRepository, IMJLogOtherRepository
    {
        private readonly ILogger<MJLogOtherRepository> _logger;
        public MJLogOtherRepository(ILogger<MJLogOtherRepository> logger, string ConnectionString)
        {
            _logger = logger;
            base.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 获取用户活动元数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<UserActionModel> GetUserActions(DateTime start,DateTime end)
        {
            try
            {
                var res = _db.Query<UserActionModel>($"select * from log_userAction where date <'{end}' and date>='{start}' ").ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserActions:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取昨日概况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<OverallSituationModel> GetSituation(int areaid,int type)
        {
            try
            {
                //昨天日期
                var time = DateTime.Now.AddDays(-1).Date;
                string sql = $"select * from log_overall_situation where areaid={areaid} and type={type}  and dataTime='{time}'";
                var res = _db.Query<OverallSituationModel>(sql).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetSituation:{e.Message}");
                throw;
            }
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public long Insert<T>(List<T> list)
        {
            try
            {
                long res = _db.Insert(list);
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"Inster:{e.Message}");
                throw;
            }
        }
        /// <summary>
        /// 修改大厅参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateAreaParams(AreaParamsModel list)
        {
            try
            {
                bool res = _db.Update(list);
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateAreaParams:{e.Message}");
                throw;
            }
        }
        /// <summary>
        /// 近期情况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<OverallSituationModel> GetThirtyDaysData(int areaid,int type,DateTime time)
        {
            try
            {
                string sql = $"select * from log_overall_situation where areaid={areaid} and type={type} and dataTime >='{time}'";
                var res = _db.Query<OverallSituationModel>(sql).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetSituation:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public AreaParamsModel GetAreaParams(int areaid,int type)
        {
            try
            {
                string sql = $"select * from log_area_param where areaid={areaid} and type={type}";
                var res = _db.QueryFirstOrDefault<AreaParamsModel>(sql);
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaParams:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> GetVersion(int areaid)
        {
            try
            {
                string sql = $"select version from log_userAction where areaid ={areaid} group by version";
                var res = _db.Query<string>(sql).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetVersion:{e.Message}");
                throw;
            }
        }
        /// <summary>
        /// user action查询
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<UserActionModel> GetActionData(int areaid,string condition)
        {
            try
            {
                string sql = $"select * from log_userAction where areaid={areaid} {condition}";
                var res = _db.Query<UserActionModel>(sql).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaParams:{e.Message}");
                throw;
            }
        }
    }
}