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
using searchpd.UI;

namespace searchpd.Controllers
{
    public class SearchController : ApiController
    {
        private const string CategoryPageUrl = "/Home/Categories/{0}";

        private readonly ISearcher _searcher = null;
        private readonly IDisplayFormatter _displayFormatter = null;

        public SearchController(ISearcher searcher, IDisplayFormatter displayFormatter)
        {
            _searcher = searcher;
            _displayFormatter = displayFormatter;
        }

        // GET api/search?q=substring
        public string GetBySubstring(string q)
        {
            string subString = q;

            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring(subString);

            var html = new StringBuilder();

            foreach (CategoryHierarchy hierarchy in hierarchies)
            {
                html.AppendFormat(@"<div class=""suggestion"">");

                html.AppendFormat(_displayFormatter.HighlightedAnchor(
                    hierarchy.Category.Name, subString, CategoryPageUrl, hierarchy.Category.CategoryID));

                if (hierarchy.Parent != null)
                {
                    html.AppendFormat(" in ");

                    html.AppendFormat(_displayFormatter.HighlightedAnchor(
                        hierarchy.Parent.Name, subString, CategoryPageUrl, hierarchy.Parent.CategoryID));
                }

                html.AppendFormat(@"</div>");
            }

            return html.ToString();
        }
    }
}

