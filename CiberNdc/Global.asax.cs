using System;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using CiberNdc.Context;
using Devtalk.EF.CodeFirst;

namespace CiberNdc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
        //    var conf = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        //    const string datacontext = "DataContext";
        //    var connstring = conf.ConnectionStrings.ConnectionStrings[datacontext].ConnectionString;
        //    if (connstring.Contains("MultipleActiveResultSets=True;")) return;
        //    connstring += "MultipleActiveResultSets=True;";
        //    conf.ConnectionStrings.ConnectionStrings[datacontext].ConnectionString = connstring;
        //    conf.Save();
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("Mobile")
            {
                ContextCondition = (context => context.GetOverriddenUserAgent()
                    .IndexOf("Opera Mobi", StringComparison.OrdinalIgnoreCase) >= 0)
            });
            //BundleTable.Bundles.RegisterTemplateBundles();
            BundleTable.Bundles.EnableDefaultBundles();
        }
    }
}