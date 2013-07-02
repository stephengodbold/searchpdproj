using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web;
using searchpd.Models;
using searchpd.Search;

namespace searchpd.Controllers
{
    public class SearchController : ApiController
    {
        private readonly ISearcher _searcher = null;

        public SearchController(ISearcher searcher)
        {
            _searcher = searcher;
        }

        // GET api/search/subString
        public string GetBySubstring(string subString)
        {
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring(subString);

            var html = new StringBuilder();

            foreach (CategoryHierarchy hierarchy in hierarchies)
            {
                html.AppendFormat(@"<div class=""suggestion"">");

                html.AppendFormat(@"<a href=""/Home/Categories/{0}"">{1}</a>",
                    hierarchy.Category.CategoryID, HttpUtility.HtmlEncode(hierarchy.Category.Name));

                if (hierarchy.Parent != null)
                {
                    html.AppendFormat(" in ");

                    html.AppendFormat(@"<a href=""/Home/Categories/{0}"">{1}</a>",
                        hierarchy.Parent.CategoryID, HttpUtility.HtmlEncode(hierarchy.Parent.Name));
                }

                html.AppendFormat(@"</div>");
            }

            return html.ToString();
        }
    }
}

