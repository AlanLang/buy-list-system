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

        public ActionResult BuyCommit(Model.Buy.BuyNew buy)
        {
            if (string.IsNullOrEmpty(buy.BuyName))
            {
                return FailResult(1, "名称不能为空");
            }
            if (buy.BuyPrice <= 0)
            {
                return FailResult(1, "价格必须大于0");
            }
            if (string.IsNullOrEmpty(buy.BuyCheckPerson))
            {
                return FailResult(1, "审核人不能为空");
            }
            using (EFContext db = new EFContext())
            {
                var checkPerson = db.SysUser.Where(m => m.UserName.Equals(buy.BuyCheckPerson)).FirstOrDefault();
                if (checkPerson == null || string.IsNullOrEmpty(checkPerson.UserCode))
                {
                    return FailResult(1, "无法识别的审核人");
                }
                var us = db.SysUser.Where(m => m.UserWx.Equals(buy.BuyAuthor)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{buy.BuyAuthor}");
                    return FailResult(1, $"无法识别的用户名{buy.BuyAuthor}");
                }
                buy.BuyState = 0;
                buy.Create = DateTime.Now;
                buy.Modified = DateTime.Now;
                buy.BuyAuthor = us.UserName;
                db.BuyNew.Add(buy);
                db.SaveChanges();
            }
            return SuccessResult("添加成功");
        }

        public ActionResult GetTypes()
        {
            using (EFContext db = new EFContext())
            {
                var types = db.DefBuyType.ToList();
                return SuccessResult(types);
            }
        }

        public ActionResult GetLevels()
        {
            var levels = new Model.Def.DefBuyLevel().GetBuyLevels();
            return SuccessResult(levels);
        }

        public ActionResult GetPersons(string user)
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var us = db.SysUser.Where(m => m.UserWx.Equals(user)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{user}");
                    return FailResult(1, $"无法识别的用户名{user}");
                }
                var persons = db.SysUser.Where(m => !m.UserCode.Equals("admin") && !m.UserCode.Equals("alan") && !m.UserCode.Equals(us.UserCode)).ToList();
                return SuccessResult(persons);
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
                string person = us.UserName;
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