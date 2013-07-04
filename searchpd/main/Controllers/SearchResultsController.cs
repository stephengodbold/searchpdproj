using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using searchpd.Models;
using searchpd.Search;

namespace main.Controllers
{
    public class SearchResultsController : Controller
    {
        private readonly IProductSearcher _searcher;

        public SearchResultsController(IProductSearcher searcher)
        {
            _searcher = searcher;
        }

        //
        // GET: /SearchResults/

        public string Index(string q)
        {
            string searchTerm = q;

            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm(searchTerm);

            var html = new StringBuilder();

            foreach (ProductSearchResult result in results)
            {
                html.AppendFormat(@"<div class=""searchresult"">");
                html.AppendFormat(result.ToHtml(searchTerm));
                html.AppendFormat(@"</div>");
            }

            string finalHtml = html.ToString();
            return finalHtml;
        }

    }
}
