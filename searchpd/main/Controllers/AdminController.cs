using System.Web.Mvc;
using main.Models;

namespace main.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConstants _constants;

        public AdminController(IConstants constants)
        {
            _constants = constants;
        }

        public ActionResult Index()
        {
            // Form will post to other server, so this server will not get the POST

            ViewBag.AutocompleteRefreshApiUrl = _constants.AutocompleteRefreshApiUrl;
            return View();
        }
    }
}
