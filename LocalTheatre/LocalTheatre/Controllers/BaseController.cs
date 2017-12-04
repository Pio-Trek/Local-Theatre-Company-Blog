using System.Web.Mvc;
using LocalTheatre.Models;

namespace LocalTheatre.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext Db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            Db.Dispose();
            base.Dispose(disposing);
        }
    }
}