using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CliffPortfolio.Helpers;
using CliffPortfolio.Models;

namespace CliffPortfolio.Controllers
{
    public class CliffBlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: CliffBlogPosts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: CliffBlogPosts/Details/5

        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CliffBlogPost cliffBlogPosts = db.Posts.FirstOrDefault(p => p.Slug == Slug); 
            if (cliffBlogPosts == null)
            {
                return HttpNotFound();
            }
            return View(cliffBlogPosts);
        }

        // GET: CliffBlogPosts/Create
        [Authorize(Roles = "Admin, Moderator")]


        public ActionResult Create()
        {
            return View();
        }

        // POST: CliffBlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Body,MediaUrl,Published,Slug")] CliffBlogPost cliffBlogPosts, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(cliffBlogPosts.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(cliffBlogPosts);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(cliffBlogPosts);
                }


                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName); //clicked light bulb for using System.IO;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName)); //clicked light bulb for using System.IO;
                    cliffBlogPosts.MediaURL = "/Uploads/" + fileName; //change URL to Url??
                }

                cliffBlogPosts.Slug = Slug;
                cliffBlogPosts.Created = DateTime.Now;
                db.Posts.Add(cliffBlogPosts);
                //db.Posts.Add(image);  // do I need this line?  I added, seems like I do but get squiggles under image
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliffBlogPosts);
        }

        // GET: CliffBlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CliffBlogPost cliffBlogPosts = db.Posts.Find(id);
            if (cliffBlogPosts == null)
            {
                return HttpNotFound();
            }
            return View(cliffBlogPosts);
        }

        // POST: CliffBlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Body,MediaUrl,Published,Slug")] CliffBlogPost cliffBlogPosts, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName); //clicked light bulb for using System.IO;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName)); //clicked light bulb for using System.IO;
                    cliffBlogPosts.MediaURL = "/Uploads/" + fileName;
                } //change URL to Url

                db.Entry(cliffBlogPosts).State = EntityState.Modified;
                //db.Posts.Add(image);  // do I need this line?  I added, seems like I do but get squiggles under image
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cliffBlogPosts);
        }

        // GET: CliffBlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CliffBlogPost cliffBlogPosts = db.Posts.Find(id);
            if (cliffBlogPosts == null)
            {
                return HttpNotFound();
            }
            return View(cliffBlogPosts);
        }

        // POST: CliffBlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CliffBlogPost cliffBlogPosts = db.Posts.Find(id);
            db.Posts.Remove(cliffBlogPosts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
