using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using searchpd.Models;
using searchpd.Search;

namespace main.Controllers
{
    public class SearchResultsController : Controller
    {
        private readonly IProductSearcher _searcher;
        private const int NbrResultsPerPage = 10;

        public SearchResultsController(IProductSearcher searcher)
        {
            _searcher = searcher;
        }

        //
        // GET: /SearchResults/

        public string Index(string q, int skip)
        {
            string searchTerm = q;

            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm(searchTerm, skip, NbrResultsPerPage, out totalHits);

            var html = new StringBuilder();

            bool showPrev = skip > 0;
            bool showNext = (skip + NbrResultsPerPage) < totalHits;

            int showingStart = skip + 1;
            int showingEnd = Math.Min(skip + NbrResultsPerPage, totalHits);
            html.AppendFormat("<p>Total results: {0} | Showing {1} to {2}</p>", totalHits, showingStart, showingEnd);

            // ---------
            
            html.Append("<p>");

            if (showPrev)
            {
                html.Append(PrevNextLink("prev", "Previous", skip - NbrResultsPerPage, searchTerm));
            }

            if (showPrev && showNext)
            {
                html.Append(" | ");
            }

            if (showNext)
            {
                html.Append(PrevNextLink("next", "Next", skip + NbrResultsPerPage, searchTerm));
            }

            html.Append("</p>");

            // --------

            foreach (ProductSearchResult result in results)
            {
                html.AppendFormat(@"<div class=""searchresult"">");
                html.AppendFormat(result.ToHtml(searchTerm));
                html.AppendFormat(@"</div>");
            }

            string finalHtml = html.ToString();
            return finalHtml;
        }

        private string PrevNextLink(string id, string linkName, int skip, string searchTerm)
        {
            string html = string.Format(@"<a id=""{0}"" href=""/SearchResults?q={3}&skip={2}"">{1}</a>",
                id, linkName, skip, HttpUtility.UrlEncode(searchTerm));

            return html;
        }
    }
}
