using sliontek_web;
using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using StackExchange.Profiling;
using EntityFramework.Extensions;
using System.Web.Security;
using System.Data.SqlClient;

namespace sliontek_web.Controllers
{
    /// <summary>
    /// 系统类控制器
    /// 创建者：郎文达
    /// 创建时间：2018年1月23日
    /// </summary>
    [StatisticsTracker]
    public class HomeController : SlionControllercs
    {
        #region 系统登陆与退出
        /// <summary>
        /// 登陆系统
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult LoginUser(string username,string userpwd)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                var user = db.SysUser.Where(m => m.UserCode == username).FirstOrDefault();
                if (user == null)
                {
                    return FailResult(1, "系统不存在此用户");
                }
                if (user.UserPwd != Cryptography.MD5Encrypt64(userpwd))
                {
                    return FailResult(1, "密码输入错误");
                }
            }
            LoggerHelper.Info("登陆系统", username);
            Response.Cookies.Add(new CookUser(username).Cookies);
            FormsAuthentication.SetAuthCookie("admin", false);
            string RefUrl;
            try
            {
                RefUrl = Request.QueryString["ReturnUrl"].Trim().ToLower();
            }
            catch { RefUrl = ""; }
            if (!string.IsNullOrEmpty(Request.Form["referurl"]))
                RefUrl = Request.Form["referurl"].ToLower();
            else
                RefUrl = "/";
            return SuccessResult(RefUrl);
        }

        /// <summary>
        /// 登出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            string rurl = Request.Url.AbsolutePath;
            int CookiesCount = Request.Cookies.Count;
            for (int i = 0; i < CookiesCount; i++)
            {
                HttpCookie cookie = Request.Cookies[i];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            Session.Clear();
            string cusercode = (string)HttpContext.Session["cookuser"];
            LoggerHelper.Info("退出登陆", new cuser().usercode);
            return View("Login");
        }

        public ActionResult GetUserNow()
        {
            return SuccessResult(new cuser().username);
        }
        public ActionResult ChangUserPass()
        {
            return View();
        }
        public ActionResult ChangThisPwd(string npwd,string oldpwd)
        {
            string usercode = new cuser().usercode;
            using (EFContext db = new Repositories.EFContext())
            {
                var user = db.SysUser.Where(m => m.UserCode == usercode).FirstOrDefault();
                if (user.UserPwd != Cryptography.MD5Encrypt64(oldpwd))
                {
                    return FailResult(1, "原始密码输入错误");
                }
                user.UserPwd = Cryptography.MD5Encrypt64(npwd);
                db.SaveChanges();
            }
            return SuccessResult("密码修改成功，请重新登陆");
        }
        #endregion

        #region 角色管理
        /// <summary>
        /// 角色管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SysRole()
        {
            return View();
        }

        /// <summary>
        /// 分页获取角色列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PageSysRole()
        {
            var profiler = MiniProfiler.Current;
            /// 调试注释
            using (profiler.Step("分页查询角色信息")) {
                using (EFContext db = new EFContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                    // var sysRoles = db.SysRole.SearchPage(Request.Form, out PageCount).Include(t => t.SysRoleMenuLimit).Include(t => t.SysUserRole).ToList();
                    var sysRoles = db.SysRole.SearchPage(Request.Form, out PageCount).ToList();
                    return PageResult(sysRoles, PageCount);
                }
            }
        }

        /// <summary>
        /// 获取一条角色信息
        /// </summary>
        /// <param name="RoleID">角色id</param>
        /// <returns></returns>
        public ActionResult GetSysRole(int RoleID)
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var SysRole = db.SysRole.Where(m => m.RoleID == RoleID).FirstOrDefault();
                return SuccessResult(SysRole);
            }
        }

        /// <summary>
        /// 删除一条角色信息
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        public ActionResult DeleteSysRole(int id)
        {
            using (EFContext db = new EFContext())
            {
                var SysRole = db.SysRole.Where(m => m.RoleID == id).FirstOrDefault();
                db.SysRole.Remove(SysRole);
                db.SysUserRole.Where(m => m.RoleID == id).Delete();//删除用户角色关联表
                db.SysRoleMenuLimit.Where(m => m.RoleID == id).Delete();//删除用户菜单功能关联表
                db.SaveChanges();
                LoggerHelper.Info("删除角色:" + SysRole.RoleName, new cuser().usercode);
                return SuccessResult("删除成功");
            }
        }

        /// <summary>
        /// 提交角色编辑
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public ActionResult EditSysRole(Model.SysRole role)
        {
            using (EFContext db = new EFContext())
            {
                if (role.RoleID > 0)
                {
                    bool HasRole = db.SysRole.Any(m => m.RoleCode == role.RoleCode && m.RoleID != role.RoleID);
                    if (HasRole)
                    {
                        return FailResult(1, "系统已有此代码的角色，请修改角色代码");
                    }
                    var dbSysRole = db.SysRole.Where(m => m.RoleID == role.RoleID).FirstOrDefault();
                    dbSysRole.RoleModified = DateTime.Now;
                    dbSysRole.RoleName = role.RoleName;
                    dbSysRole.RoleCode = role.RoleCode;
                    dbSysRole.RoleDesc = role.RoleDesc;
                    db.SaveChanges();
                    return SuccessResult("修改成功");
                }
                else
                {
                    bool HasRole = db.SysRole.Any(m => m.RoleCode == role.RoleCode);
                    if (HasRole)
                    {
                        return FailResult(1, "系统已有此代码的角色，请修改角色代码");
                    }
                    db.SysRole.Add(role);
                    role.RoleCreate = DateTime.Now;
                    role.RoleModified = DateTime.Now;
                    LoggerHelper.Info("新增角色：" + GetContent(role), new cuser().usercode);
                    db.SaveChanges();
                    return SuccessResult("添加成功");
                }
            }
        }

        /// <summary>
        /// 角色分配菜单页面
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        public ActionResult SysRoleMenu(int id)
        {
            using (EFContext db = new EFContext())
            {
                //db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var menu = db.SysMenu.Include(m=>m.SysMenuLimit).OrderBy(m=>m.MenuSort).ToList();
                var aa = menu.ToList();
                var roleMenuLimit = db.SysRoleMenuLimit.Where(m => m.RoleID == id).ToList();
                ViewBag.id = id;
                ViewBag.menus = menu;
                ViewBag.roleMenuLimit = roleMenuLimit;
            }
            List<Model.SysRoleMenuLimit> limits = new List<Model.SysRoleMenuLimit>();
            limits.Where(m => m.RoleMenuLimitID == 1).Count();
            return View();
        }

        /// <summary>
        /// 设置角色菜单权限表
        /// </summary>
        /// <param name="roleid">角色id</param>
        /// <param name="limitid">菜单功能id</param>
        /// <param name="type">类型 1 新增，0 删除</param>
        /// <returns></returns>
        public ActionResult SetSysRoleMenuLimit(int roleid, int limitid, int type)
        {
            try
            {
                using (EFContext db = new EFContext())
                {
                    if (type == 1)
                    {
                        Model.SysRoleMenuLimit rml = new Model.SysRoleMenuLimit()
                        {
                            RoleID = roleid,
                            MenuLimitID = limitid,
                            RoleMenuModified = DateTime.Now,
                            RoleMenuCreate = DateTime.Now
                        };
                        db.SysRoleMenuLimit.Add(rml);
                        db.SaveChanges();
                    }
                    else
                    {
                        var rml = db.SysRoleMenuLimit.Where(m => m.RoleID == roleid && m.MenuLimitID == limitid).FirstOrDefault();
                        db.SysRoleMenuLimit.Remove(rml);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return FailResult(1, ex.Message);
            }
            return SuccessResult("修改成功");
        }
        #endregion

        #region 菜单管理
        /// <summary>
        /// 系统菜单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SysMenu()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var menus = db.SysMenu.Where(m => m.MenuFa == 0).ToList();
                ViewData["MenuFa"] = new SelectList(menus.AsEnumerable(), "MenuID", "MenuName");
            }
            return View();
        }

        /// <summary>
        /// 分页查询菜单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PageSysMenu()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var SysMenu = db.SysMenu.WhereDynamic(Request.Form).OrderBy(m=>m.MenuSort).ToList();
                return PageResult(SysMenu, PageCount);
            }
        }

        /// <summary>
        /// 菜单排序
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <param name="type">up/down</param>
        /// <returns></returns>
        public ActionResult MenuSort(int id,string type)
        {
            using (EFContext db = new EFContext())
            {
                var SysMenu = db.SysMenu.Where(m=>m.MenuID == id).FirstOrDefault();
                var index = SysMenu.MenuSort;
                if ("up".Equals(type)) // 如果是向上移动
                {
                    if (SysMenu.MenuFa == 0)
                    {
                        if (index == 1)
                        {
                            return FailResult(1, "该菜单已处于最顶端，移动失败。");
                        }
                        var menuFaUp = db.SysMenu.Where(m => m.MenuSort < index && m.MenuFa == 0).OrderBy(m => m.MenuSort).FirstOrDefault();
                        int leeIndex = index - menuFaUp.MenuSort;
                        db.Database.ExecuteSqlCommand($"update SysMenu set MenuSort = MenuSort - {leeIndex} where MenuID = {SysMenu.MenuID} or MenuFa = {SysMenu.MenuID}");
                        db.Database.ExecuteSqlCommand($"update SysMenu set MenuSort = {leeIndex} + MenuSort where MenuID = {menuFaUp.MenuID} or MenuFa = {menuFaUp.MenuID}");
                    }
                    else
                    {
                        var menuUp = db.SysMenu.Where(m => m.MenuSort == (index - 1)).FirstOrDefault();
                        if (menuUp.MenuFa == 0)
                        {
                            return FailResult(1, "该菜单已处于最顶端，移动失败。");
                        }
                        else
                        {
                            menuUp.MenuSort = index;
                            SysMenu.MenuSort = index - 1;
                            db.SaveChanges();
                        }
                    }
                }
                else // 如果是向下移动
                {
                    if (SysMenu.MenuFa == 0) // 如果是移动主菜单
                    {
                        var menuFaDown = db.SysMenu.Where(m => m.MenuSort > index && m.MenuFa == 0).OrderByDescending(m => m.MenuSort).FirstOrDefault();
                        if (menuFaDown == null)
                        {
                            return FailResult(1, "该菜单已处于最底端，移动失败。");
                        }
                        int addIndex = menuFaDown.MenuSort - index;
                        db.Database.ExecuteSqlCommand($"update SysMenu set MenuSort = MenuSort + {addIndex} where MenuID = {SysMenu.MenuID} or MenuFa = {SysMenu.MenuID}");
                        db.Database.ExecuteSqlCommand($"update SysMenu set MenuSort = MenuSort - {addIndex} where MenuID = {menuFaDown.MenuID} or MenuFa = {menuFaDown.MenuID}");
                    }
                    else // 如果是移动子菜单
                    {
                        var MenuDown = db.SysMenu.Where(m => m.MenuSort == (index + 1)).FirstOrDefault();
                        if (MenuDown == null || MenuDown.MenuFa == 0)
                        {
                            return FailResult(1, "该菜单已处于最底端，移动失败。");
                        }
                        MenuDown.MenuSort = index;
                        SysMenu.MenuSort = index + 1;
                        db.SaveChanges();
                    }
                }
                return SuccessResult("移动成功");
            }
        }

        /// <summary>
        /// 菜单功能页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SysMenuBars(int id)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                bool hasBar = db.SysMenuLimit.Where(m => m.MenuID == id && "view".Equals(m.MenuLimitCode)).Any();
                if (!hasBar)
                {
                    AddMenuViewBar(id);
                }
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var mlModels = db.SysMenuLimit.Where(m => m.MenuID == id && !"view".Equals(m.MenuLimitCode)).OrderBy(m=>m.MenuLimitSort).ToList();
                ViewBag.model = mlModels;
            }
            ViewBag.id = id;
            return View();
        }

        /// <summary>
        /// 编辑菜单功能
        /// </summary>
        /// <returns></returns>
        public ActionResult SysMenuBarsEdit()
        {
            string[] codes = Request.Form["MenuLimitCode"].Split(',');
            string[] names = Request.Form["MenuLimitName"].Split(',');
            string[] sorts = Request.Form["MenuLimitSort"].Split(',');
            string id = Request.Form["id"];
            if (codes.Length != names.Length)
            {
                return FailResult(1,"功能代码和功能名称数量不匹配！");
            }
            if (codes.Length != sorts.Length)
            {
                return FailResult(1, "功能代码和排序数量不匹配！");
            }
            int sort = 0;
            for (int i = 0; i < sorts.Length; i++)
            {
                if (!Int32.TryParse(sorts[i],out sort))
                {
                    return FailResult(1, "排序自动只能是数字");
                }
            }
            using (EFContext db = new Repositories.EFContext())
            {
                var dbContextTransaction = db.Database.BeginTransaction();
                try
                {
                    // 删除菜单权限
                    db.Database.ExecuteSqlCommand("delete SysRoleMenuLimit where MenuLimitID in (select MenuLimitID from SysMenuLimit where MenuLimitCode <> 'view' and MenuID = {0})", id);
                    db.Database.ExecuteSqlCommand("delete SysMenuLimit where MenuLimitCode <> 'view' and MenuID = {0}", id);
                    for (int i = 0; i < codes.Length; i++)
                    {
                        Model.SysMenuLimit mlModel = new Model.SysMenuLimit()
                        {
                            MenuID = Int32.Parse(id),
                            MenuLimitModified = DateTime.Now,
                            MenuLimitCreate = DateTime.Now,
                            MenuLimitCode = codes[i],
                            MenuLimitName = names[i],
                            MenuLimitSort = Int32.Parse(sorts[i])
                        };
                        db.SysMenuLimit.Add(mlModel);
                    }
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return FailResult(1, ex.Message);
                }
                finally {
                    dbContextTransaction.Dispose();
                }
            return SuccessResult("保存成功");
            }
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public ActionResult EidtSysMenu(Model.SysMenu menu)
        {
            bool IsAddBar = false;
            using (EFContext db = new EFContext())
            {
                if (menu.MenuID > 0)
                {
                    var dbSysMenu = db.SysMenu.Where(m => m.MenuID == menu.MenuID).FirstOrDefault();
                    dbSysMenu.MenuModified = DateTime.Now;
                    dbSysMenu.MenuName = menu.MenuName;
                    dbSysMenu.MenuUrl = menu.MenuUrl;
                    dbSysMenu.MenuDesc = menu.MenuDesc;
                    dbSysMenu.MenuFa = menu.MenuFa;
                    db.SaveChanges();
                    return SuccessResult("菜单修改成功");
                }
                else
                {
                    if (menu.MenuFa == 0)//如果提交的是主菜单
                    {
                        var lastFa = db.SysMenu.OrderByDescending(m => m.MenuSort).FirstOrDefault();
                        menu.MenuSort = lastFa.MenuSort + 1;
                    }
                    else
                    {
                        var lastMenu = db.SysMenu.Where(m => m.MenuFa == menu.MenuFa).OrderByDescending(m => m.MenuSort).FirstOrDefault();
                        if (lastMenu == null)
                        {
                            lastMenu = db.SysMenu.Where(m => m.MenuID == menu.MenuFa).FirstOrDefault();
                        }
                        menu.MenuSort = lastMenu.MenuSort + 1;
                        IsAddBar = true;
                    }
                    db.Database.ExecuteSqlCommand("update SysMenu set MenuSort = MenuSort + 1 where MenuSort > {0}", menu.MenuSort - 1);
                    db.SysMenu.Add(menu);
                    menu.MenuCreate = DateTime.Now;
                    menu.MenuModified = DateTime.Now;
                    LoggerHelper.Info("新增菜单：" + GetContent(menu), new cuser().usercode);
                    db.SaveChanges();
                    if (IsAddBar)
                    {
                        AddMenuViewBar(menu.MenuID);
                    }
                    return SuccessResult("菜单添加成功");
                }
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteSysMenu(int id)
        {
            using (EFContext db = new EFContext())
            {
                var menu = db.SysMenu.Where(m => m.MenuID == id).FirstOrDefault();
                if (menu.MenuFa == 0)
                {
                    bool hasChild = db.SysMenu.Where(m => m.MenuFa == id).Any();
                    if (hasChild)
                    {
                        return FailResult(1,"请先删除子菜单");
                    }
                }
                db.Database.ExecuteSqlCommand("delete SysRoleMenuLimit where MenuLimitID in (select MenuLimitID from SysMenuLimit where MenuID = {0})", id);
                db.SysMenuLimit.Where(m => m.MenuID == id).Delete();
                db.Database.ExecuteSqlCommand("update SysMenu set MenuSort = MenuSort - 1 where MenuSort > {0}", menu.MenuSort - 1);
                db.SysMenu.Where(m => m.MenuID == id).Delete();
                db.SaveChanges();
                return SuccessResult("菜单删除成功");
            }
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        public ActionResult GetMenu(int id)
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var menu = db.SysMenu.Where(m => m.MenuID == id).FirstOrDefault();
                return SuccessResult(menu);
            }
        }

        /// <summary>
        /// 获取用户权限下的菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult ListMenusByUser()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                List<Model.SysMenuNavbar.navbar> nvbs = new List<Model.SysMenuNavbar.navbar>();
                if ("alan".Equals(new cuser().usercode))
                {
                    nvbs = db.Database.SqlQuery<Model.SysMenuNavbar.navbar>("select MenuID as pmid,MenuName as title from SysMenu where MenuFa = 0 order by MenuSort").ToList();
                    foreach (var item in nvbs)
                    {
                        item.children = db.Database.SqlQuery<Model.SysMenuNavbar.children>("select menuid as pmid,menuname as title, MenuUrl as href from SysMenu where MenuFa = {0} order by MenuSort",item.pmid).ToList();
                    }
                    return Json(nvbs, JsonRequestBehavior.AllowGet);
                }
                else {
                    string usercode = new cuser().usercode;
                    
                    string sqlMenuFa = "select SysMenu.MenuID as pmid,SysMenu.MenuName as title from SysMenu where MenuFa = 0 and MenuID in (select distinct SysMenu.MenuFa from SysMenu left join SysMenuLimit on SysMenu.MenuID = SysMenuLimit.MenuID inner join SysRoleMenuLimit on SysMenuLimit.MenuLimitID = SysRoleMenuLimit.MenuLimitID left join SysRole on SysRole.RoleID = SysRoleMenuLimit.RoleID left join SysUserRole on SysUserRole.RoleID = SysRole.RoleID left join SysUser on SysUserRole.UserID = SysUser.UserID where UserCode = @usercode and MenuLimitCode = 'view') order by MenuSort ";
                    nvbs = db.Database.SqlQuery<Model.SysMenuNavbar.navbar>(sqlMenuFa, new SqlParameter("@usercode", usercode)).ToList();
                    foreach (var item in nvbs)
                    {
                        string sqlMenu = " select distinct SysMenu.MenuID as pmid,SysMenu.MenuName as title, SysMenu.MenuUrl as href,MenuSort  from SysMenu left join SysMenuLimit on SysMenu.MenuID = SysMenuLimit.MenuID inner join SysRoleMenuLimit on SysMenuLimit.MenuLimitID = SysRoleMenuLimit.MenuLimitID left join SysRole on SysRole.RoleID = SysRoleMenuLimit.RoleID left join SysUserRole on SysUserRole.RoleID = SysRole.RoleID left join SysUser on SysUserRole.UserID = SysUser.UserID where SysMenu.MenuFa = @id and MenuLimitCode = 'view' and UserCode = @usercode order by MenuSort";
                        item.children = db.Database.SqlQuery<Model.SysMenuNavbar.children>(sqlMenu, new SqlParameter("@id", item.pmid), new SqlParameter("@usercode", usercode)).ToList();
                    }
                    return Json(nvbs, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 获取用户权限下的菜单功能
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult ListMenuLimitByUser(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return FailResult(1, "请求地址不可为空");
            }
            using (EFContext db = new EFContext())
            {
                if ("alan".Equals(new cuser().usercode))
                {
                    var menu = db.SysMenu.Where(m => url.ToLower().Equals(m.MenuUrl.ToLower())).FirstOrDefault();
                    if (menu == null)
                    {
                        return FailResult(1, "无法识别的菜单:" + url);
                    }
                    var menuLimit = db.SysMenuLimit.Where(m => m.MenuID == menu.MenuID && !"view".Equals(m.MenuLimitCode)).OrderBy(m => m.MenuLimitSort).ToList();
                    return SuccessResult(menuLimit);
                }
                else
                {
                    var menu = db.SysMenu.Where(m => url.ToLower().Equals(m.MenuUrl.ToLower())).FirstOrDefault();
                    if (menu == null)
                    {
                        return FailResult(1, "无法识别的菜单:" + url);
                    }
                    //var menuLimit = db.SysMenuLimit.Where(m => m.MenuID == menu.MenuID && !"view".Equals(m.MenuLimitCode)).OrderBy(m => m.MenuLimitSort).ToList();
                    string usercode = new cuser().usercode;
                    var menuLimit = db.Database.SqlQuery<Model.SysMenuLimit>("select distinct MenuLimitCode,MenuLimitName,SysMenuLimit.MenuLimitID,SysMenuLimit.MenuID,MenuLimitSort,MenuLimitModified,MenuLimitCreate from SysMenuLimit  inner join SysRoleMenuLimit on SysMenuLimit.MenuLimitID = SysRoleMenuLimit.MenuLimitID left join SysRole on SysRole.RoleID = SysRoleMenuLimit.RoleID left join SysUserRole on SysUserRole.RoleID = SysRole.RoleID left join SysUser on SysUserRole.UserID = SysUser.UserID where UserCode = @usercode and MenuLimitCode <> 'view' and MenuID = @id order by MenuLimitSort", new SqlParameter("@usercode", usercode), new SqlParameter("@id", menu.MenuID)).ToList();
                    return SuccessResult(menuLimit);
                }
            }

        }
        #endregion

        #region 用户管理
        /// <summary>
        /// 用户管理主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SysUser()
        {
            return View();
        }

        /// <summary>
        /// 分页获取用户数据
        /// </summary>
        /// <returns></returns>
        public ActionResult PageSysUser()
        {
            using (EFContext db = new EFContext())
            {
                db.Configuration.LazyLoadingEnabled = false;//禁用懒加载
                var sysUser = db.SysUser.SearchPage(Request.Form, out PageCount).Where(m=>!"alan".Equals(m.UserCode)).ToList();
                return PageResult(sysUser, PageCount);
            }
        }
        /// <summary>
        /// 删除一条用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteSysUser(int id)
        {
            using (EFContext db = new EFContext())
            {
                var user = db.SysUser.Where(m => m.UserID == id).FirstOrDefault();
                if ("admin".Equals(user.UserCode))
                {
                    return FailResult(1, "admin用户不可删除");
                }
                db.SysUser.Remove(user);
                db.SysUserRole.Where(m => m.UserID == id).Delete();// 删除用户时是删除用户与角色关联表
                db.SaveChanges();
                LoggerHelper.Info("删除用户：" + GetContent(user), new cuser().usercode);
            }
            return SuccessResult("用户删除成功");
        }
        /// <summary>
        /// 获取一条用户信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public ActionResult GetSysUser(int id)
        {
            using (EFContext db = new EFContext())
            {
                var user = db.SysUser.Where(m => m.UserID == id).FirstOrDefault();
                return SuccessResult(user);
            }
        }
        /// <summary>
        /// 新增或修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult EditSysUser(Model.SysUser user)
        {
            using (EFContext db = new EFContext())
            {
                if (user.UserID > 0)//如果是编辑
                {
                    bool HasUser = db.SysUser.Any(m => m.UserCode == user.UserCode && m.UserID != user.UserID);
                    if (HasUser)
                    {
                        return FailResult(1, $"系统已有代码为【{user.UserCode}】的用户，请修改用户代码");
                    }
                    var dbSysUser = db.SysUser.Where(m => m.UserID == user.UserID).FirstOrDefault();
                    dbSysUser.UserModified = DateTime.Now;
                    dbSysUser.UserName = user.UserName;
                    dbSysUser.UserCode = user.UserCode;
                    dbSysUser.UserMail = user.UserMail;
                    db.SaveChanges();
                    return SuccessResult("用户修改成功");
                }
                else {
                    bool HasUser = db.SysUser.Any(m => m.UserCode == user.UserCode);
                    if (HasUser)
                    {
                        return FailResult(1, $"系统已有代码为【{user.UserCode}】的用户，请修改用户代码");
                    }
                    user.UserCreate = DateTime.Now;
                    user.UserModified = DateTime.Now;
                    user.UserPwd = Cryptography.MD5Encrypt64(user.UserPwd);
                    db.SysUser.Add(user);
                    LoggerHelper.Info("新增用户：" + GetContent(user), new cuser().usercode);
                    db.SaveChanges();
                    return SuccessResult("添加成功");
                }
            }
        }
        /// <summary>
        /// 用户角色分配页面
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public ActionResult SysUserRole(int id)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                var Roles = db.SysRole.ToList();
                var userRoles = db.SysUserRole.Where(m => m.UserID == id).ToList();
                ViewBag.Roles = Roles;
                ViewBag.userRoles = userRoles;
                ViewBag.id = id;
            }
            return View();
        }
        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="roleid">角色id</param>
        /// <param name="userid">用户id</param>
        /// <param name="type">类型：1添加；0删除</param>
        /// <returns></returns>
        public ActionResult SetSysUserRole(int roleid, int userid, int type)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                if (type == 1)
                {
                    Model.SysUserRole userRole = new Model.SysUserRole()
                    {
                        UserID = userid,
                        RoleID = roleid,
                        UserRoleCreate = DateTime.Now,
                        UserRoleModified = DateTime.Now
                    };
                    db.SysUserRole.Add(userRole);
                    db.SaveChanges();
                }
                else
                {
                    var userRole = db.SysUserRole.Where(m => m.UserID == userid && m.RoleID == roleid).FirstOrDefault();
                    db.SysUserRole.Remove(userRole);
                    db.SaveChanges();
                }
            }
            return SuccessResult("修改成功");
        }

        public ActionResult ChangePwd(int id, string npwd)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                var user = db.SysUser.Where(m => m.UserID == id).FirstOrDefault();
                user.UserPwd = Cryptography.MD5Encrypt64(npwd);
                db.SaveChanges();
            }
            return SuccessResult("密码修改成功");
        }
        #endregion

        #region 公用方法
        /// <summary>
        /// 给菜单添加查看权限的bar
        /// 当新增一个菜单的时候，给菜单添加一个查询的功能。
        /// </summary>
        /// <param name="id"></param>
        protected void AddMenuViewBar(int id)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                Model.SysMenuLimit limit = new Model.SysMenuLimit()
                {
                    MenuID = id,
                    MenuLimitModified = DateTime.Now,
                    MenuLimitCreate = DateTime.Now,
                    MenuLimitCode = "view",
                    MenuLimitName = "查看",
                    MenuLimitSort = 0
                };
                db.SysMenuLimit.Add(limit);
                db.SaveChanges();
            }
        }
        #endregion

        // GET: Home
        public ActionResult Index()
        {
            using (EFContext db = new Repositories.EFContext())
            {
                var SysRole = db.SysRole.Take(3).ToList();
                Json(SysRole, JsonRequestBehavior.AllowGet);
                return View();
            }
        }
        public ActionResult Getmac(string ip = "192.168.1.35")
        {
            string s = ApiHelp.GetIP(HttpContext);
            string mac = ApiHelp.GetMacAddress(ip);
            return Content("192.168.1.35的mac地址是："+mac+",系统获取的ip是："+s);
        }
    }
}