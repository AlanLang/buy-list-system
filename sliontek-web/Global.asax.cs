using sliontek_web.Repositories;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static sliontek_web.Repositories.EFContext;

namespace sliontek_web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MiniProfilerEF6.Initialize();
        }
        protected void Application_BeginRequest()
        {
            //#if DEBUG
            //MiniProfiler.Start();
            //#endif
        }
        protected void Application_EndRequest()
        {
            //#if DEBUG
            //MiniProfiler.Stop();
            //#endif
        }
    }
}
