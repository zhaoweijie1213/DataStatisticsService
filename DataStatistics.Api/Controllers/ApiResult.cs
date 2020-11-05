using DataStatistics.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataStatistics.Api.Controllers
{
    /// <summary>
    /// 返回结果对象
    /// ApiResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public int Code { get; set; } = (int)HttpStatusCode.OK;
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; } = "操作成功";
        /// <summary>
        /// 结果
        /// </summary>
        public T Result { get; set; } = default;


        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="result">返回结果</param>
        /// <param name="msg">提示消息</param>
        public ApiResult<T> Success(T result, string msg = "操作成功")
        {
            this.Code = (int)ResultCode.Success;
            this.Result = result;
            this.Message = msg;
            return this;
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="msg">提示消息</param>
        /// <param name="code"></param>
        public ApiResult<T> Error(string msg = "操作失败", int code = (int)ResultCode.Error)
        {
            this.Result = default;
            this.Code = code;
            this.Message = msg;
            return this;
        }
    }
}
