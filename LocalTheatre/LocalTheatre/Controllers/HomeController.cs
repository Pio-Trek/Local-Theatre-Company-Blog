using System.Linq;
using System.Web.Mvc;

namespace LocalTheatre.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var model = Db.BlogPosts.ToList();

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}