using System.Web.Mvc;
using Shop.Models;

namespace Shop.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext _db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}