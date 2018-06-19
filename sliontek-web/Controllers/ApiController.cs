using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web.Controllers
{
    [MemberCheck]
    public class ApiController : SlionControllercs
    {
        public ActionResult GetBuyList(string user)
        {
            using (EFContext db = new EFContext())
            {
                var us = db.SysUser.Where(m => m.UserWx.Equals(user)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{user}");
                    return FailResult(1,$"无法识别的用户名{user}");
                }
                LoggerHelper.Info($"api-用户{user}获取列表");
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var re = db.BuyNew.Where(m => m.BuyState == 1 && m.BuyCheckPerson.Contains(us.UserName)).ToList();
                if (re.Count == 0)
                {
                    return FailResult(1, "无数据");
                }
                return SuccessResult(re);
            }
        }

        public ActionResult BuyCheckCommit(int id, int type, string log,string user)
        {
            using (EFContext db = new EFContext())
            {
                var us = db.SysUser.Where(m => m.UserWx.Equals(user)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{user}");
                    return FailResult(1, $"无法识别的用户名{user}");
                }
                string person = us.UserCode;
                Model.Buy.BuyNewChangeLog cLog = new Model.Buy.BuyNewChangeLog()
                {
                    BuyNewID = id,
                    LogStatus = type,
                    Create = DateTime.Now,
                    LogMsg = log,
                    Person = person,
                    ChangeFrom = "微信"
                };
                db.BuyNewChangeLog.Add(cLog);
                var buy = db.BuyNew.Where(m => m.ID == id).FirstOrDefault();
                if (type == 1)
                {
                    buy.BuyState = 3;
                }
                else
                {
                    buy.BuyState = 2;
                }
                db.SaveChanges();
            }
            return SuccessResult("提交成功");
        }
    }
}