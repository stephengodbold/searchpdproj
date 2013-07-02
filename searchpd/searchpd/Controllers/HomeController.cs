using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using searchpd.Repositories;

namespace searchpd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository = null;

        public HomeController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories(int id)
        {
            Category category = _categoryRepository.GetCategoryById(id);
            ViewBag.CategoryName = (category == null) ? "Unkown" : category.Name;

            return View();
        }
    }
}
