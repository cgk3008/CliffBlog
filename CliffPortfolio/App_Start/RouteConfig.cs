using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CliffPortfolio
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

 //           routes.MapRoute(
 //name: "NewPostId",
 //url: "CliffBlogPosts/Details/{slug}",
 //defaults: new { controller = "CliffBlogPosts", action = "Details", slug = UrlParameter.Optional });


 //           {
 //    controller = "BlogPosts",
 //    action = "Details",
 //    slug = UrlParameter.Optional
 //});


            routes.MapRoute(
                name: "NewSlug", 
                url: "CliffBlogPosts/Details/{slug}", 
                defaults: new { controller = "CliffBlogPosts", action = "Details", slug = UrlParameter.Optional });           

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "CliffBlogPosts", action = "Index", id = UrlParameter.Optional });
        }
    }
}
