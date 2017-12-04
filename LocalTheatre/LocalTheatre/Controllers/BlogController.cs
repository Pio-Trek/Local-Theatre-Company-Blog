using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LocalTheatre.Models;

namespace LocalTheatre.Controllers
{
    public class BlogController : BaseController
    {
        // GET: Blog
        public ActionResult Index()
        {
            return View(Db.BlogPosts.OrderByDescending(m => m.PublishDate).ToList());
        }

        // GET: Blog/Create
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Create(
            [Bind(Include = "Id,Title,Article,PublishDate,AuthorId,CoverUrl,CoverFileName")] Blog model)
        {
            if (ModelState.IsValid)
            {
                Db.BlogPosts.Add(model);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Details(int? id, bool? confirm)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var blogPost = Db.BlogPosts.Find(id);
            if (blogPost == null)
                return HttpNotFound();

            if (confirm != null && confirm == true)
            {
                ViewBag.Confirmation = Message.CommentAwaitForApprove;
            }

            return View(blogPost);
        }

        // GET: Blog/Edit/5
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = Db.BlogPosts.Find(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Edit(Blog model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Modified;
                Db.SaveChanges();

                if (Request.Form["submitAndGoToPost"] != null)
                {
                    return RedirectToAction("Details", new {id = model.Id});
                }
                if (Request.Form["deleteAndRefresh"] != null)
                {
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: Blog/Delete/5
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = Db.BlogPosts.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Delete(int id)
        {
            var model = Db.BlogPosts.Find(id);
            Db.BlogPosts.Remove(model ?? throw new InvalidOperationException());
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}