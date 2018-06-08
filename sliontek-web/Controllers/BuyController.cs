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
        // GET: Buy
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
                var persons = db.SysUser.Where(m => m.UserCode != "alan" && m.UserCode != userCode).ToList();
                return SuccessResult(persons);
            }
        }

        public ActionResult PageBuyNew() {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var re = db.BuyNew.SearchPage(Request.Form, out PageCount).ToList();
                return PageResult(re, PageCount);
            }
        }
        public ActionResult EditBuyNew(Model.Buy.BuyNew model)
        {
            model.BuyState = 0;
            model.Create = DateTime.Now;
            model.Modified = DateTime.Now;
            using (EFContext db = new EFContext())
            {
                db.BuyNew.Add(model);
                db.SaveChanges();
            }
            return SuccessResult("添加成功");
        }
    }
}