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
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("Opera")
            {
                ContextCondition = context =>
                    context.Request.UserAgent != null &&
                    context.Request.UserAgent.IndexOf("Opera", StringComparison.OrdinalIgnoreCase) >= 0
            });
            //BundleTable.Bundles.RegisterTemplateBundles();
            BundleTable.Bundles.EnableDefaultBundles();

            var conf = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var connstring = conf.ConnectionStrings.ConnectionStrings["DataContext"].ConnectionString;
            if (connstring.Contains("MultipleActiveResultSets=True;")) return;
            connstring += "MultipleActiveResultSets=True;";
            conf.ConnectionStrings.ConnectionStrings["FootyFeudContext"].ConnectionString = connstring;
            conf.Save();
        }
    }
}