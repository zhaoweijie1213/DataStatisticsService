using Dapper;
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
        public List<UserActionModel> GetUserActions(DateTime start, DateTime end)
        {
            try
            {
                using (_db)
                {
                    var res = _db.Query<UserActionModel>($"select type,date,areaid,uid,version,data,platForm from log_userAction where date <'{end}' and date>='{start}' ").ToList();
                    return res;
                }
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
        public List<OverallSituationModel> GetSituation(int areaid, int type)
        {
            try
            {
                using (_db)
                {
                    //昨天日期
                    var time = DateTime.Now.AddDays(-1).Date;
                    string sql = $"select * from log_overall_situation where areaid={areaid} and type={type}  and dataTime='{time}'";
                    var res = _db.Query<OverallSituationModel>(sql).ToList();
                    return res;
                }

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
                using (_db)
                {
                    long res = _db.Insert(list);
                    return res;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Inster:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool Delete<T>(List<T> list)
        {
            try
            {
                using (_db)
                {
                    bool res = _db.Delete(list);
                    return res;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete:{e.Message}");
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
                using (_db)
                {
                    bool res = _db.Update(list);
                    return res;
                }
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
        public List<OverallSituationModel> GetThirtyDaysData(int areaid, int type, DateTime time)
        {
            try
            {
                using (_db)
                {
                    string sql = $"select * from log_overall_situation where areaid={areaid} and type={type} and dataTime >='{time}'";
                    var res = _db.Query<OverallSituationModel>(sql).ToList();
                    return res;
                }
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
        public AreaParamsModel GetAreaParams(int areaid, int type)
        {
            try
            {
                using (_db)
                {
                    string sql = $"select * from log_area_param where areaid={areaid} and type={type}";
                    var res = _db.QueryFirstOrDefault<AreaParamsModel>(sql);
                    return res;
                }
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
            string sql = "";
            try
            {
                using (_db)
                {
                    var time = DateTime.Now.Date.AddYears(-1);
                    sql = $"SELECT version FROM log_userAction where areaid={areaid} and date>'{time}' GROUP BY version";
                    var res = _db.Query<string>(sql).ToList();
                    return res;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetVersion:{e.Message},sql:{sql}");
                throw;
            }
        }
        /// <summary>
        /// user action查询
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<UserActionModel> GetActionData(int areaid, string condition)
        {
            try
            {
                using (_db)
                {
                    //string sql = $"select type,date,areaid,uid,version,data,platForm from log_userAction where areaid={areaid} {condition}";
                    //不查data
                    string sql = $"select type,date,areaid,uid,version,platForm from log_userAction where areaid={areaid} {condition}";
                    var res = _db.Query<UserActionModel>(sql).ToList();
                    return res;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaParams:{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 大厅版本号
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        public List<AreaVersion> GetAreaVersion(int areaid)
        {
            try
            {
                using (_db)
                {
                    string sql = $"select areaid,version from log_area_version where areaid={areaid}";
                    var res = _db.Query<AreaVersion>(sql).ToList();
                    return res;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaVersion:{e.Message}");
                return null;
            }
        }
        /// <summary>
        /// 检查是否存在该版本
        /// </summary>
        /// <returns></returns>
        public List<AreaVersion> CheckAreaVersion(int areaid, string version)
        {
            try
            {
                using (_db)
                {
                    string sql = $"select id,areaid,version from log_area_version where areaid={areaid} and version='{version}'";
                    var res = _db.Query<AreaVersion>(sql).ToList();
                    return res;
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError($"CheckAreaVersion:{e.Message}");
                return null;
            }
        }
        /// <summary>
        /// 删除大厅版本
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllAreaVersion()
        {
            try
            {
                using (_db)
                {
                    string sql = $"delete from log_area_version where areaid is not null";
                    var res = _db.Execute(sql);
                    return res > 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetAreaVersion:{e.Message}");
                return false;
            }
        }
        /// <summary>
        /// 查询当天行为统计
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="type"></param>
        /// <param name="platForm"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public UserActionStatisticsModel QueryByContion(int areaid,int uid,int type,string platForm,string version,string uuid)
        {
            UserActionStatisticsModel model = new UserActionStatisticsModel();
            if (string.IsNullOrEmpty(platForm))
            {
                platForm = " ";
            }
            if (string.IsNullOrEmpty(version))
            {
                version = " ";
            }
            string sql = $"select * from log_userActionStatistics where areaid={areaid}  and type={type} and platForm='{platForm}' and version='{version}' and uid={uid} and uuid='{uuid}'";
            try
            {
                using (_db)
                {
                    model = _db.QueryFirstOrDefault<UserActionStatisticsModel>(sql);
                    return model;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"QueryByContion:{e.Message},sql语句:{sql}");
                return model;
            }
        }
        /// <summary>
        /// 日行为分析列表
        /// </summary>
        /// <returns></returns>
        public List<UserActionStatisticsModel> QueryUserActStat(int areaid,DateTime start,DateTime end)
        {
            List<UserActionStatisticsModel> list = new List<UserActionStatisticsModel>();
            string sql = $"select id,areaid,type,platForm,uuid,uid,added,version from log_userActionStatistics where areaid={areaid} and added  between '{start}' and  '{end}' ";
            try
            {
                using (_db)
                {
                    list = _db.Query<UserActionStatisticsModel>(sql).ToList();
                    return list;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"QueryUserActStat:{e.Message},sql语句:{sql}");
                return list;
            }
        }

        /// <summary>
        /// 删除日行为分析数据
        /// </summary>
        /// <returns></returns>
        public bool DeleteUserActStat(DateTime end)
        {
            bool res = false;
            string sql = $"delete from log_userActionStatistics where  added < '{end}' ";
            try
            {
                using (_db)
                {
                    var count = _db.Execute(sql);
                    return count>0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DeleteUserActStat:{e.Message},sql语句:{sql}");
                return res;
            }
        }
        /// <summary>
        /// 删除概况
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public  bool DeleteYeatodayData(int areaid,DateTime time)
        {
            bool res = false;
            string sql = $"delete from log_overall_situation where  areaid={areaid} adn dataTime='{time}' ";
            try
            {
                using (_db)
                {
                    var count = _db.Execute(sql);
                    return count > 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DeleteUserActStat:{e.Message},sql语句:{sql}");
                return res;
            }
        }
    }
}
