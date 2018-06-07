using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web.Controllers
{
    public class DefineController : SlionControllercs
    {
        #region 购物类型

        public ActionResult BuyType()
        {
            return View();
        }

        public ActionResult PageBuyType()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var sysUser = db.DefBuyType.SearchPage(Request.Form, out PageCount).ToList();
                return PageResult(sysUser, PageCount);
            }
        }

        public ActionResult DeleteBuyType(int id)
        {
            using (EFContext db = new EFContext())
            {
                var buyType = db.DefBuyType.Where(m => m.ID == id).FirstOrDefault();
                db.DefBuyType.Remove(buyType);
                db.SaveChanges();
                return SuccessResult("删除成功");
            }
        }

        public ActionResult EditBuyType(Model.Def.DefBuyType buyType)
        {
            using (EFContext db = new EFContext())
            {
                buyType.Modified = DateTime.Now;
                buyType.Create = DateTime.Now;
                db.DefBuyType.Add(buyType);
                db.SaveChanges();
                return SuccessResult("新增成功");
            }
        }
        #endregion
    }
}