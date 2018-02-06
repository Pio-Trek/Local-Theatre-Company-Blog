using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using LocalTheatre.Models;

namespace LocalTheatre.Controllers
{
    public class CommentController : BaseController
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultipleButtonAttribute : ActionNameSelectorAttribute
        {
            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName,
                MethodInfo methodInfo)
            {
                var isValidName = false;
                var keyValue = $"{Name}:{Argument}";
                var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                if (value != null)
                {
                    controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                    isValidName = true;
                }

                return isValidName;
            }
        }

        // GET: Comment/Create
        [Authorize]
        public ActionResult _AddComment(int? id)
        {
            // If sending request without 'id' parametr
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // If 'id' parametr is out of scope
            var blogPost = Db.BlogPosts.Find(id);
            if (blogPost == null)
                return HttpNotFound();

            var model = new Comment {BlogId = (int) id};

            return View(model);
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult _AddComment([Bind(Include = "Text,AuthorId,PublishDate,BlogId")] Comment model)
        {
            if (ModelState.IsValid)
            {
                Db.Comments.Add(model);
                Db.SaveChanges();
                ViewBag.BlogId = model.BlogId;
                return PartialView("_CommentConfirmation");
            }

            return View(model);
        }

        // GET: Comment/List
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Moderate(string search = null)
        {
            var model = Db.Comments.Where(c => c.IsModerated.Equals(false)).OrderByDescending(c => c.PublishDate)
                .ToList();

            if (!string.IsNullOrEmpty(search))
                model = model.Where(c => c.Text.Contains(search)).ToList();

            return View(model);
        }

        // POST: Comment/Moderate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        [MultipleButton(Name = "action", Argument = "Approve")]
        public ActionResult Moderate(List<Comment> comments)
        {
            foreach (var itemComment in comments)
            {
                var currentComment = Db.Comments.Find(itemComment.Id);

                Debug.Assert(currentComment != null, nameof(currentComment) + " != null");

                currentComment.Id = itemComment.Id;
                currentComment.Text = itemComment.Text;
                currentComment.AuthorId = itemComment.AuthorId;
                currentComment.PublishDate = itemComment.PublishDate;
                currentComment.BlogId = itemComment.BlogId;
                currentComment.IsModerated = itemComment.IsModerated;
            }

            Db.SaveChanges();

            return RedirectToAction("Moderate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteFromList(List<Comment> comments)
        {
            foreach (var itemComment in comments)
            {
                var currentComment = Db.Comments.Find(itemComment.Id);

                Debug.Assert(currentComment != null, nameof(currentComment) + " != null");

                if (itemComment.DeleteFromListHelpColumn)
                    Db.Comments.Remove(currentComment);
            }

            Db.SaveChanges();

            return RedirectToAction("Moderate");
        }

        // GET: Comment/Delete/5
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = Db.Comments.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Writer)]
        public ActionResult Delete(int id)
        {
            var model = Db.Comments.Find(id);
            Db.Comments.Remove(model ?? throw new InvalidOperationException());
            Db.SaveChanges();
            return RedirectToAction("Details", "Blog", new {id = model.BlogId});
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