using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web.Controllers
{
    public class BuyController : SlionControllercs
    {
        #region 购物申请
        public ActionResult BuyNew()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var buyTypes = db.DefBuyType.ToList();
                var buyLevels = new Model.Def.DefBuyLevel().GetBuyLevels();
                ViewData["buyTypes"] = new SelectList(buyTypes.AsEnumerable(), "TypeName", "TypeName");
                ViewData["buyLevels"] = new SelectList(buyLevels.AsEnumerable(), "BuyLevel", "BuyLevel");
            }
            return View();
        }

        public ActionResult GetPersons()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                string userCode = new cuser().usercode;
                var persons = db.SysUser.Where(m => !m.UserCode.Equals("admin") && !m.UserCode.Equals("alan") && !m.UserCode.Equals(userCode)).ToList();
                return SuccessResult(persons);
            }
        }

        public ActionResult PageBuyNew() {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var re = db.BuyNew.Where(m=>m.BuyState < 4).SearchPage(Request.Form, out PageCount).ToList();
                return PageResult(re, PageCount);
            }
        }
        public ActionResult EditBuyNew(Model.Buy.BuyNew model)
        {
            if (string.IsNullOrEmpty( model.BuyCheckPerson))
            {
                return FailResult(1, "审批人不能为空");
            }
            if (model.ID > 0)
            {
                using (EFContext db = new EFContext())
                {
                    var buyNew = db.BuyNew.Where(m => m.ID == model.ID).FirstOrDefault();
                    buyNew.BuyName = model.BuyName;
                    buyNew.BuyPrice = model.BuyPrice;
                    buyNew.BuyUrl = model.BuyUrl;
                    buyNew.BuyTypeName = model.BuyTypeName;
                    buyNew.BuyLevel = model.BuyLevel;
                    buyNew.BuyCheckPerson = model.BuyCheckPerson;
                    buyNew.BuyTime = model.BuyTime;
                    buyNew.BuyDesc = model.BuyDesc;
                    buyNew.Modified = DateTime.Now;
                    db.SaveChanges();
                    return SuccessResult("修改成功");
                }
            }
            else {
                model.BuyState = 0;
                model.Create = DateTime.Now;
                model.Modified = DateTime.Now;
                model.BuyAuthor = new cuser().username;
                using (EFContext db = new EFContext())
                {
                    db.BuyNew.Add(model);
                    db.SaveChanges();
                }
                return SuccessResult("添加成功");
            }

        }
        public ActionResult BuyNewCommit(int id)
        {
            BuyNewUpdateStatus(id, 1);
            return SuccessResult("已提交审核");
        }
        public ActionResult BuyGiveUp(int id)
        {
            BuyNewUpdateStatus(id, 4);
            return SuccessResult("成功放弃购买");
        }
        public ActionResult BuyArchive(int id)
        {
            BuyNewUpdateStatus(id, 5);
            return SuccessResult("成功归档");
        }
        public ActionResult GetBuyNew(int id)
        {
            using (EFContext db = new EFContext())
            {
                var buyNew = db.BuyNew.Where(m => m.ID == id).FirstOrDefault();
                return SuccessResult(buyNew);
            }
        }
        protected void BuyNewUpdateStatus(int id, int status)
        {
            using (EFContext db = new EFContext())
            {
                var buyNew = db.BuyNew.Where(m => m.ID == id).FirstOrDefault();
                buyNew.BuyState = status;
                db.SaveChanges();
            }
        }
        #endregion

        #region 购物审核
        public ActionResult BuyCheck()
        {
            return View();
        }
        public ActionResult PageBuyCheck()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var re = db.BuyNew.Where(m => m.BuyState == 1).SearchPage(Request.Form, out PageCount).ToList();
                return PageResult(re, PageCount);
            }
        }
        public ActionResult BuyCheckCommit(int id, int type,string log)
        {
            string person = new cuser().username;
            using (EFContext db = new EFContext())
            {
                Model.Buy.BuyNewChangeLog cLog = new Model.Buy.BuyNewChangeLog()
                {
                    BuyNewID = id,
                    LogStatus = type,
                    Create = DateTime.Now,
                    LogMsg = log,
                    Person = person

                };
                db.BuyNewChangeLog.Add(cLog);
                var buy = db.BuyNew.Where(m => m.ID == id).FirstOrDefault();
                if (type == 1)
                {
                    buy.BuyState = 3;
                }
                else {
                    buy.BuyState = 2;
                }
                db.SaveChanges();
            }
            return SuccessResult("提交成功");
        }

        #endregion
        
        public ActionResult BuyCheckLog(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult PageCheckLog(int id)
        {
            using (EFContext db = new EFContext())
            {
                var log = db.BuyNewChangeLog.Where(m => m.BuyNewID == id).OrderByDescending(m=>m.Create).ToList();
                return SuccessResult(log);
            }
        }
    }
}