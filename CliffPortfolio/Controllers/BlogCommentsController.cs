using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CliffPortfolio.Models;
using Microsoft.AspNet.Identity;

namespace CliffPortfolio.Controllers
{
    [RequireHttps]
    public class BlogCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogComments
        public ActionResult Index()
        {
            var blogComments = db.Comments.Include(b => b.Author).Include(b => b.Post);
            return View(blogComments.ToList());
        }

        // GET: BlogComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.Comments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        // GET: BlogComments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title");
            return View();
        }

        // POST: BlogComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,Body")] BlogComment blogComment)
        {

            //return to details page!!!!! once logged in, set a return URL?  or use HTTP requester to go back to previous page (in this case details page)  , new { ReturnUrl = ViewBag.ReturnUrl }
            if (ModelState.IsValid)
            {
                blogComment.AuthorId = User.Identity.GetUserId();
                blogComment.Created = DateTime.Now;

                db.Comments.Add(blogComment);
                db.SaveChanges();
                var bp = db.Comments.Include("Post").FirstOrDefault(c => c.Id == blogComment.Id);


                if (User.IsInRole("Admin, Moderator"))
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    return RedirectToAction("Details", "CliffBlogPosts", new { slug = bp.Post.Slug });
                }

                /*"Details", "CliffBlogPosts"*//*, new { id = Comments.PostId }*/
                                               //}
                                               //else return RedirectToAction("Details", "CliffBlogPosts", new { id = db.Posts, blogComment
                                               //.PostId});

                //return Url.Action("Details", "CliffBlogPosts", new { id = "PostId" })
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", blogComment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", blogComment.PostId);
            return View(blogComment);
        }

        // GET: BlogComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.Comments.Find(id);
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", blogComment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", blogComment.PostId);
            return View(blogComment);
        }

        // POST: BlogComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,AuthorId,Body,Created,Updated,UpdateReason")] BlogComment blogComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", blogComment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", blogComment.PostId);
            return View(blogComment);
        }

        // GET: BlogComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment blogComment = db.Comments.Find(id); /* removed "Blog from "BlogComments" don't want the CLass, we want the a property, scaffolding tool, and did this with a slight hiccup in property name.*/
            if (blogComment == null)
            {
                return HttpNotFound();
            }
            return View(blogComment);
        }

        // POST: BlogComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogComment blogComment = db.Comments.Find(id);
            db.Comments.Remove(blogComment);
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
