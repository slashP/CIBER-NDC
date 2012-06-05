using System;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using CiberNdc.Context;
using CiberNdc.Util;
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

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },// Parameter defaults
                new[] { "RebusMobile.Controllers" }
            );

        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DisplayModeProvider.Instance.Modes.Insert(0, new MobileDisplayMode());
            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("Mobile")
            {
                ContextCondition = context =>
                    context.GetOverriddenBrowser().IsMobileDevice
            }); 

            BundleTable.Bundles.EnableDefaultBundles();
        }
    }
}