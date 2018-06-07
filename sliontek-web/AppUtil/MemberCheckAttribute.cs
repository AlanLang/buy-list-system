using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MemberCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断是否登录或是否用权限，如果有那么就进行相应的操作，否则跳转到登录页或者授权页面
            string imei = filterContext.HttpContext.Request["imei"];
            if (string.IsNullOrEmpty(imei))
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        code = 1,
                        err = "请输入唯一识别码"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string msg = "\r\n 请求参数:"+ filterContext.RouteData.Values["controller"] as string + "/" + filterContext.RouteData.Values["action"] as string + "?" + filterContext.HttpContext.Request.QueryString + filterContext.HttpContext.Request.Form;
            foreach (var item in filterContext.Result.GetType().GetProperties())
            {
                object n = item.GetValue(filterContext.Result, null);
                string name = item.Name.ToLower();
                if (n != null && name == "content")
                {
                    string re = Newtonsoft.Json.JsonConvert.SerializeObject(n, Newtonsoft.Json.Formatting.None);
                    if (!string.IsNullOrEmpty(re))
                    {
                        re = re.Replace("\\r", "").Replace("\\n", "").Replace("\\\"", "").Replace(" ", "");
                        msg += "\r\n 返回结果：" + re;
                    }
                    LoggerHelper.api(msg);
                }
            }
        }
    }
    /// <summary>
    /// 异常持久化类
    /// </summary>
    public class ExceptionLogAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 触发异常时调用的方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            string ControllerName = string.Format("{0}Controller", filterContext.RouteData.Values["controller"] as string);
            string ActionName = filterContext.RouteData.Values["action"] as string;
            string Eurl = filterContext.RouteData.Values["controller"] as string + "/" + filterContext.RouteData.Values["action"] as string + "?" + filterContext.HttpContext.Request.QueryString + filterContext.HttpContext.Request.Form;
            string ErrorMsg = string.Format("在执行 controller[{0}] 的 action[{1}] 时产生异常，请求为：[{2}]", ControllerName, ActionName, Eurl);
            ErrorMsg += "\n异常消息为：" + filterContext.Exception;
            LoggerHelper.api(ErrorMsg);
        }
    }
}