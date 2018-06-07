using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web.Controllers
{
    public class BuyController : Controller
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
    }
}