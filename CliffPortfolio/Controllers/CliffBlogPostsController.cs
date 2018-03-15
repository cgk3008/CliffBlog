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
using PagedList;
using PagedList.Mvc;

namespace CliffPortfolio.Controllers
{
    [RequireHttps]
    public class CliffBlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: CliffBlogPosts 
        //[HttpPost] this was only used on the Post action which I have removed.
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            //IQueryable<CliffBlogPost> listPosts = db.Posts.AsQueryable();
            //var listPosts = db.Posts.AsQueryable();
            //OrderByDescending
            return View(blogList./*OrderByDescending(p => p.Created).*/ToPagedList(pageNumber, pageSize));

        }

        public IQueryable<CliffBlogPost> IndexSearch(string searchStr)
        {
            IQueryable<CliffBlogPost> result = null;
            if (searchStr != null)
            {
                result = db.Posts.AsQueryable();

                result = result.Where(p => p.Body.Contains(searchStr))
                                .Union(db.Posts.Where(p => p.Title.Contains(searchStr)))
                                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(searchStr))))
                                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(searchStr))))
                                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(searchStr))))
                                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(searchStr))))
                                 //.Union(db.Posts.Where(p => p.Comments.Any(c => c.Email.Contains(searchStr))))
                                 .Union(db.Posts.Where(p => p.Comments.Any(c => c.UpdateReason.Contains(searchStr))));
            }
            else
            {
                result = db.Posts.AsQueryable();
            }
            return /*View(*/result.OrderByDescending(p => p.Created)/*.ToPagedList(pageNumber, pageSize))*/;
        }
        //var listPosts = db.Posts.AsQueryable(); 
        //thought i needed db.CliffBlogPosts not db.Posts...see IdentityModels...set database value of a blog post to Posts  ---  public DbSet<CliffBlogPost> Posts { get; set; }

        //listPosts = listPosts.Where(p => p.Title.Contains(searchStr)) ||
        //    p.Body.Contains(searchStr) ||
        //    p.Comments.Any(c => c.Body.Contains(searchStr) ||
        //                           c.Author.FirstName.Contains(searchStr) ||
        //                           c.Author.DisplayName.Contains(searchStr) ||
        //                           c.Author.LastName.Contains(searchStr) ||
        //                           c.Email.Contains(searchStr) ||
        //                           c.UpdateReason.Contains(searchStr));

        //Also, when search then click on blog post from search, go to details, should I put a back to search button/text link? same as back to list/index

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
        [Authorize(Roles = "Admin")]


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
            CliffBlogPost cliffBlogPosts = db.Posts.Find(id); //.FirstOrDefault (finds first of whatever name or number searching for, see PowerPoint) rather than .Find if we don't have id.  delete all records with string or id, use .Where
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
