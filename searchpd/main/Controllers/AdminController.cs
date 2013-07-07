using System.Web.Mvc;
using main.Models;

namespace main.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
