using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace searchpd.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new searchpdEntities())
            {
                ViewBag.datalist = context.Categories.Take(10).ToList();
            }

            return View();
        }
    }
}
