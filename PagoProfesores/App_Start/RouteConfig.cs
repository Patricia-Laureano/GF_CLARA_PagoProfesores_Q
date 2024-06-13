using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PagoProfesores
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                  name: "Default",
                  url: "{controller}/{action}/{id_token}",
                  defaults: new { controller = "SSOffice365", action = "Start", id_token = UrlParameter.Optional }
              );

        }
    }
}
