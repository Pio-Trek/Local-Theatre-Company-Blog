using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shop.Models;

namespace Shop.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string search = null)
        {
            //var selectCategories2 = _db.Categories.Where(c => c.Name.Length < 7 && c.Id < 2).Take(10);

            IEnumerable<Category> model;

            if (!string.IsNullOrEmpty(search))
                model = _db.Categories.Where(c => c.Name.Contains(search)).ToList();
            else
                model = _db.Categories.ToList();

            //var model = _db.Categories.ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}