using DataStatistics.Model.mj_log_other;
using DataStatistics.Service.Repositorys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.Impl
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IMJLogOtherRepository _repository;
        public DataService(IMJLogOtherRepository repository, ILogger<DataService> logger)
        {
            _logger = logger;
            _repository = repository;
        }
        public List<UserActionModel> GetUserActions()
        {
            try
            {
                var res = _repository.GetUserActions();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetUserActions:{e.Message}");
                throw;
            }
        }
    }
}
