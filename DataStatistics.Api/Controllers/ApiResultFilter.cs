using DataStatistics.Api.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStatistics.Api.Controllers
{
    /// <summary>
    /// 返回结果重新封装
    /// </summary>
    public class ApiResultFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                ApiResult<object> result = new ApiResult<object>();
                string message = "参数错误,";
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        message += error.ErrorMessage + ",";
                    }
                }

                //var reader = new StreamReader(context.HttpContext.Request.Body);
                //var contentFromBody = reader.ReadToEnd();


                message = message.TrimEnd(',');
                result.Error(message, (int)ResultCode.ErrorParams);
                ObjectResult objectResult = new ObjectResult(result);
                context.Result = objectResult;
            }
            base.OnResultExecuting(context);
        }
        /// <summary>
        /// Action执行完成,返回结果处理
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
            {

            }
            else
            {
                ApiResult<object> robj = new ApiResult<object>();
                robj.Error(actionExecutedContext.Exception.Message, actionExecutedContext.HttpContext.Response.StatusCode);
                ObjectResult objectResult = new ObjectResult(robj);
                actionExecutedContext.Result = objectResult;
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
