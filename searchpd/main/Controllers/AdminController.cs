using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using main.Models;
using searchpd;
using searchpd.Repositories;

namespace main.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConstants _constants = null;

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
