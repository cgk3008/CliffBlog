using System;
using CliffPortfolio.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace CliffPortfolio.Controllers
{
   [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();

            //return RedirectToAction("Index", "CliffBlogPosts");
        }



        public ActionResult ComingSoon()
        {
            return View();
        }

        //POST: Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email model)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                //Email vis = new Email();


                msg.Body = model.FromName + ", " + Environment.NewLine + model.Body + Environment.NewLine + Environment.NewLine + " Contact's email: " + model.FromEmail;



                msg.Destination = "cgk3008.ck@gmail.com";
                msg.Subject = "Portfolio message from " + model.FromName;

                await ems.SendMailAsync(msg);
            }
            catch (Exception /*ex*/)
            {
                await Task.FromResult(0);
            }

            Response.Write("<script type='text/javascript'>");
            Response.Write("alert('Email sent successfully.');");
            Response.Write("</script>");

            return View("Index");
        }

    }
}
