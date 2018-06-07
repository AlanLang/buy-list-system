using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web
{
    /// <summary>
    /// 日志工具类
    /// 创建者：郎文达
    /// 创建时间：2018年1月23日
    /// </summary>
    public class LoggerHelper
    {
        static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        static readonly log4net.ILog logmonitor = log4net.LogManager.GetLogger("logmonitor");
        static readonly log4net.ILog logapi = log4net.LogManager.GetLogger("logapi");
        public static void api(string msg)
        {
            logapi.Info(msg);
        }
        public static void Error(string ErrorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                logerror.Error(ErrorMsg, ex);
            }
            else
            {
                logerror.Error(ErrorMsg);
            }
        }
        public static void Info(string Msg)
        {
            loginfo.Info(Msg);
        }

        public static void Info(string Msg, string user)
        {
            loginfo.Info("用户【"+user+"】:"+Msg);
        }
        public static void Monitor(string Msg)
        {
            logmonitor.Info(Msg);
        }
    }

    /// <summary>
    /// 监控日志对象
    /// </summary>
    public class MonitorLog
    {
        public string UserAddress
        {
            get;
            set;
        }
        public string ControllerName
        {
            get;
            set;
        }
        public string ActionName
        {
            get;
            set;
        }

        public DateTime ExecuteStartTime
        {
            get;
            set;
        }
        public DateTime ExecuteEndTime
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public string Response
        {
            get;
            set;
        }
        /// <summary>
        /// Form 表单数据
        /// </summary>
        public NameValueCollection FormCollections
        {
            get;
            set;
        }
        /// <summary>
        /// URL 参数
        /// </summary>
        public NameValueCollection QueryCollections
        {
            get;
            set;
        }
        /// <summary>
        /// 监控类型
        /// </summary>
        public enum MonitorType
        {
            Action = 1,
            View = 2
        }
        /// <summary>
        /// 获取监控指标日志
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public string GetLoginfo(MonitorType mtype = MonitorType.Action)
        {
            string ActionView = "Action执行时间监控：";
            string Name = "Action";
            if (mtype == MonitorType.View)
            {
                ActionView = "View视图生成时间监控：";
                Name = "View";
            }
            string Msg = @"
            {0}
            ControllerName：{1}Controller
            ActionName:{2}
            开始时间：{3}
            结束时间：{4}
            总 时 间：{5}秒
            用户地址：{10}
            Form表单数据：{6}
            URL参数：{7}
            URL:{8}
            返回：{9}
                    ";
            return string.Format(Msg,
                ActionView,
                ControllerName,
                ActionName,
                ExecuteStartTime,
                ExecuteEndTime,
                (ExecuteEndTime - ExecuteStartTime).TotalSeconds,
                GetCollections(FormCollections),
                GetCollections(QueryCollections),
                Url,
                Response,
                UserAddress,
                Name);
        }

        /// <summary>
        /// 获取Post 或Get 参数
        /// </summary>
        /// <param name="Collections"></param>
        /// <returns></returns>
        public string GetCollections(NameValueCollection Collections)
        {
            string Parameters = string.Empty;
            if (Collections == null || Collections.Count == 0)
            {
                return Parameters;
            }
            foreach (string key in Collections.Keys)
            {
                Parameters += string.Format("{0}={1}&", key, Collections[key]);
            }
            if (!string.IsNullOrWhiteSpace(Parameters) && Parameters.EndsWith("&"))
            {
                Parameters = Parameters.Substring(0, Parameters.Length - 1);
            }
            return Parameters;
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class StatisticsTrackerAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly string Key = "_thisOnActionMonitorLog_";

        #region Action时间监控
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MonitorLog MonLog = new MonitorLog();
            MonLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            MonLog.ControllerName = filterContext.RouteData.Values["controller"] as string;
            MonLog.ActionName = filterContext.RouteData.Values["action"] as string;
            filterContext.Controller.ViewData[Key] = MonLog;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            MonLog.FormCollections = filterContext.HttpContext.Request.Form;//form表单提交的数据
            MonLog.QueryCollections = filterContext.HttpContext.Request.QueryString;//Url 参数
            MonLog.Url = filterContext.RouteData.Values["controller"] as string + "/" + filterContext.RouteData.Values["action"] as string + "?" + filterContext.HttpContext.Request.QueryString;
            MonLog.UserAddress = filterContext.HttpContext.Request.UserHostAddress;
            //LoggerHelper.Monitor(MonLog.GetLoginfo());
        }
        #endregion

        #region View 视图生成时间监控
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            //MonLog.ExecuteStartTime = DateTime.Now;
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            MonLog.ExecuteEndTime = DateTime.Now;
            foreach (var item in filterContext.Result.GetType().GetProperties())
            {
                object n = item.GetValue(filterContext.Result, null);
                string name = item.Name.ToLower();
                if (n != null && name == "content")
                {
                    MonLog.Response = System.Web.Helpers.Json.Encode(n);
                }
            }
            LoggerHelper.Monitor(MonLog.GetLoginfo(MonitorLog.MonitorType.View));
            filterContext.Controller.ViewData.Remove(Key);
        }
        #endregion

        #region 错误日志
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                string ControllerName = string.Format("{0}Controller", filterContext.RouteData.Values["controller"] as string);
                string ActionName = filterContext.RouteData.Values["action"] as string;
                string Eurl = filterContext.RouteData.Values["controller"] as string + "/" + filterContext.RouteData.Values["action"] as string + "?" + filterContext.HttpContext.Request.QueryString;
                string ErrorMsg = string.Format("在执行 controller[{0}] 的 action[{1}] 时产生异常，请求为：[{2}]", ControllerName, ActionName, Eurl);
                LoggerHelper.Error(ErrorMsg, filterContext.Exception);
            }
        }
        #endregion
    }
}