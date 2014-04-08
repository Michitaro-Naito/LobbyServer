using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LobbyServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // PlayLog
            routes.MapRoute(
                name: "PlayLogIndex",
                url: "{culture}/PlayLog/{page}",
                defaults: new { culture = "Auto", controller = "PlayLog", action = "Index", page = UrlParameter.Optional }
            );

            // Default
            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = UrlParameter.Optional, controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
