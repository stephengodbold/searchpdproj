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
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository = null;
        private readonly IProductRepository _productRepository = null;
        private readonly IConstants _constants = null;

        public HomeController(ICategoryRepository categoryRepository, IProductRepository productRepository,
            IConstants constants)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _constants = constants;
        }

        public ActionResult Index()
        {
            ViewBag.AutocompleteSearchApiUrl = _constants.AutocompleteSearchApiUrl;
            return View();
        }

        public ActionResult Categories(int id)
        {
            Category category = _categoryRepository.GetCategoryById(id);
            ViewBag.CategoryName = (category == null) ? "Unkown" : category.Name;

            return View();
        }

        public ActionResult Products(int id)
        {
            Product product = _productRepository.GetProductById(id);
            ViewBag.ProductDescription = (product == null) ? "Unkown" : product.Description;
            ViewBag.ProductCode = (product == null) ? "Unkown" : product.Code;

            return View();
        }
    }
}
