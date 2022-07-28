using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace API_Project
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            string projectName = Assembly.GetExecutingAssembly().GetName().Name;
            _ = new LoggerLib.Logger(HttpRuntime.AppDomainAppPath.Replace(projectName, "LoggerLib"));
            _ = new AuthDbLib.TokenCleaner();
            ServerState.State.DateStart = DateTime.Now;
        }
    }
}
