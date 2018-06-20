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
        #region 获取待审批的列表
        public ActionResult GetBuyList(string user)
        {
            using (EFContext db = new EFContext())
            {
                var us = db.SysUser.Where(m => m.UserWx.Equals(user)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{user}");
                    return FailResult(1, $"无法识别的用户名{user}");
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

        #endregion

        #region 提交购物审批
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
            if (string.IsNullOrEmpty(buy.BuyTypeName))
            {
                return FailResult(1, "物品类别不能为空");
            }
            if (string.IsNullOrEmpty(buy.BuyLevel))
            {
                return FailResult(1, "物品级别不能为空");
            }
            if (string.IsNullOrEmpty(buy.BuyTime))
            {
                return FailResult(1, "购买时间不能为空");
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
                if (buy.ID > 0)
                {
                    var buyNew = db.BuyNew.Where(m => m.ID == buy.ID).FirstOrDefault();
                    buyNew.BuyName = buy.BuyName;
                    buyNew.BuyPrice = buy.BuyPrice;
                    buyNew.BuyUrl = buy.BuyUrl;
                    buyNew.BuyTypeName = buy.BuyTypeName;
                    buyNew.BuyLevel = buy.BuyLevel;
                    buyNew.BuyCheckPerson = buy.BuyCheckPerson;
                    buyNew.BuyTime = buy.BuyTime;
                    buyNew.BuyDesc = buy.BuyDesc;
                    buyNew.Modified = DateTime.Now;
                    buyNew.BuyState = 1;
                    db.SaveChanges();
                    return SuccessResult("修改成功");
                }
                else {
                    buy.BuyState = 1;
                    buy.Create = DateTime.Now;
                    buy.Modified = DateTime.Now;
                    buy.BuyAuthor = us.UserName;
                    db.BuyNew.Add(buy);
                    db.SaveChanges();
                }
            }
            return SuccessResult("添加成功");
        }

        #endregion

        #region 获取购物类型
        public ActionResult GetTypes()
        {
            using (EFContext db = new EFContext())
            {
                var types = db.DefBuyType.ToList();
                return SuccessResult(types);
            }
        }

        #endregion

        #region 获取购物级别
        public ActionResult GetLevels()
        {
            var levels = new Model.Def.DefBuyLevel().GetBuyLevels();
            return SuccessResult(levels);
        }
        #endregion

        #region 获取可以审批的人员
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

        #endregion

        #region 提交审批结果
        public ActionResult BuyCheckCommit(int id, int type, string log, string user)
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

        #endregion

        #region 获取指定人员提交的购物申请
        public ActionResult GetMyBuyList(string wxuser)
        {
            using (EFContext db = new EFContext())
            {
                var us = db.SysUser.Where(m => m.UserWx.Equals(wxuser)).FirstOrDefault();
                if (us == null || string.IsNullOrEmpty(us.UserCode))
                {
                    LoggerHelper.Info($"无法识别的用户{wxuser}");
                    return FailResult(1, $"无法识别的用户名{wxuser}");
                }
                var buyList = db.BuyNew.Where(m => m.BuyAuthor.Equals(us.UserName) && m.BuyState < 4).ToList();
                return SuccessResult(buyList);
            }
        }
        #endregion

        #region  修改购物申请的状态
        public ActionResult UpdateBuyListType(int id, int status)
        {
            using (EFContext db = new EFContext())
            {
                var buyItem = db.BuyNew.Where(m => m.ID == id).FirstOrDefault();
                if (buyItem == null || string.IsNullOrEmpty(buyItem.BuyName))
                {
                    return FailResult(1, "无法识别的购物申请");
                }
                buyItem.BuyState = status;
                buyItem.Modified = DateTime.Now;
                db.SaveChanges();
                return SuccessResult("修改成功");
            }
        }
        #endregion

        #region 获取审核日志的最后一条
        public ActionResult GetCheckMsg(int id)
        {
            using (EFContext db = new EFContext())
            {
                var log = db.BuyNewChangeLog.Where(m => m.BuyNewID == id).OrderByDescending(m => m.ID).FirstOrDefault();
                return SuccessResult(log);
            }
        }
        #endregion

    }
}