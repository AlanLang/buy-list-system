using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace sliontek_web.Controllers
{

    public class Search
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 每页条目数
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 条件查询
        /// </summary>
        public string condition { get; set; }

        public string where { get { return getsearch(); } }

        public string orderby
        {
            get
            {
                string s = field + " " + order;
                return string.IsNullOrWhiteSpace(s) ? "" : " order by " + s;
            }
        }

        public string getsearch()
        {
            string twhere = "";
            if (string.IsNullOrEmpty(condition))
            {
                return "";
            }
            else
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<JsonCondition> list = (List<JsonCondition>)jss.Deserialize(condition, typeof(List<JsonCondition>));
                twhere = (list.Count > 0) ? GetWhere(list) : "";
                if (!string.IsNullOrWhiteSpace(twhere))
                {
                    twhere = twhere.Trim();
                    twhere = " where " + twhere.Substring(3, twhere.Length - 3);
                }
                return twhere;
            }
        }

        protected string GetWhere(List<JsonCondition> cList)
        {
            string sqlWhere = "";
            if (cList != null)
            {
                foreach (JsonCondition jc in cList)
                {
                    sqlWhere += jc.toWhereString() + " ";
                }
            }
            return sqlWhere;
        }
    }
    public class JsonNetResult
    {
        /// <summary>
        /// 返回状态 0/正常 非0为异常
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回异常信息
        /// </summary>
        public string err { get; set; }
        /// <summary>
        /// 返回data内数据条数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public Object data { get; set; }
    }

    /// <summary>
    /// 基础控制器
    /// 创建者：郎文达
    /// 创建时间：2018年1月23日
    /// </summary>
    public class SlionControllercs : Controller
    {
        public int PageCount = 0;
        public int RecordCount = 0;
        /// <summary>
        /// 返回正常的接口信息
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public ActionResult SuccessResult(object o)
        {
            int size = 0;
            if (o is IList)
                size = ((IList)o).Count;
            else
                size = 1;
            if (o is String)
            {
                JsonNetResult jr = new JsonNetResult() { code = 0, msg = o.ToString() };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
            }
            else
            {
                JsonNetResult jr = new JsonNetResult() { code = 0, data = o, count = size };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
            }
        }

        /// <summary>
        /// 返回异常消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public ActionResult FailResult(int code, string err)
        {
            JsonNetResult jr = new JsonNetResult() { code = code, err = err };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
        }
        public ActionResult FailResult(int code, string err, object o)
        {
            JsonNetResult jr = new JsonNetResult() { code = code, err = err,data = o };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
        }
        /// <summary>
        /// 返回分页数据
        /// </summary>
        /// <param name="o"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ActionResult PageResult(object o, int count)
        {
            JsonNetResult jr = new JsonNetResult() { code = 0, data = o, count = count };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
        }

        public ActionResult PageResult(object o)
        {
            JsonNetResult jr = new JsonNetResult() { code = 0, data = o, count = RecordCount };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter()));
        }

        public string GetContent(object o)
        {
            JsonNetResult jr = new JsonNetResult() { code = 0, data = o};
            return Newtonsoft.Json.JsonConvert.SerializeObject(jr, new UnixDateTimeConverter());
        }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}