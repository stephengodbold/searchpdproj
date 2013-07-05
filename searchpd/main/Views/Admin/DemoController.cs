using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using searchpd;

namespace main.Views.Admin
{
    /// <summary>
    /// This controler is not for production, only for demo purposes.
    /// </summary>
    public class DemoController : Controller
    {
        //
        // GET: /Demo/

        public ActionResult AddProduct()
        {
            var product = new Product();

            return View(product);
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            using (var context = new searchpdEntities())
            {
                product.CategoryID = 36790;
                product.NodeID = 36790;
                product.Image = "";
                context.Products.Add(product);
                context.SaveChanges();
            }

            return View(product);
        }

        public ActionResult AddCategory()
        {
            var category = new Category();

            return View(category);
        }

        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            using (var context = new searchpdEntities())
            {
                category.ParentID = 36790;
                context.Categories.Add(category);
                context.SaveChanges();
            }

            return View(category);
        }

    }
}
