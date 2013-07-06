using System.Web.Mvc;
using main.Models;

namespace main.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConstants _constants;

        public AdminController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
