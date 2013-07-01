using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web;

namespace searchpd.Controllers
{
    public class SearchController : ApiController
    {
        // GET api/search/category
        public string GetByName(string categoryName)
        {
            IEnumerable<Category> matchingCategories = null;

            using (var context = new searchpdEntities())
            {
                matchingCategories =
                    context.Categories.Where(c => c.Name.StartsWith(categoryName)).ToList();
            }

            var html = new StringBuilder();

            foreach (Category category in matchingCategories)
            {
                html.AppendFormat(@"<a href=""/Home/Categories/{0}"">{1}</a>", 
                    category.CategoryID, HttpUtility.HtmlEncode(category.Name));
            }

            return html.ToString();
        }
    }
}

