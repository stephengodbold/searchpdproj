using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using main.Models;
using searchpd.Models;
using searchpd.Search;

namespace main.Controllers
{
    public class SearchResultsController : Controller
    {
        private readonly IProductSearcher _searcher;
        private readonly IConstants _constants;

        public SearchResultsController(IProductSearcher searcher, IConstants constants)
        {
            _searcher = searcher;
            _constants = constants;
        }

        //
        // GET: /SearchResults/

        /// <summary>
        /// Returns the search results
        /// </summary>
        /// <param name="q">
        /// Search term
        /// </param>
        /// <param name="skip">
        /// Number of results to skip.
        /// 
        /// The number of search results to return (after the skip) is in _constants.NbrResultsPerPage
        /// </param>
        /// <returns>
        /// Html with search results, stats and prev/next links.
        /// </returns>
        public string Index(string q, int skip)
        {
            string searchTerm = q;

            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm(searchTerm, skip, _constants.NbrResultsPerPage, out totalHits);

            if (!results.Any())
            {
                return "<p>No results</p>";
            }

            var html = new StringBuilder();

            html.Append(StatsBoxHtml(skip, totalHits));
            html.Append(PrevNextBoxHtml(skip, totalHits, searchTerm));

            foreach (ProductSearchResult result in results)
            {
                html.AppendFormat(@"<div class=""searchresult"">");
                html.AppendFormat(result.ToHtml(searchTerm));
                html.AppendFormat(@"</div>");
            }

            string finalHtml = html.ToString();
            return finalHtml;
        }

        private string StatsBoxHtml(int skip, int totalHits)
        {
            int showingStart = skip + 1;
            int showingEnd = Math.Min(skip + _constants.NbrResultsPerPage, totalHits);
            string html = string.Format("<p>Total results: {0} | Showing {1} to {2}</p>", totalHits, showingStart, showingEnd);
            return html;
        }

        private string PrevNextBoxHtml(int skip, int totalHits, string searchTerm)
        {
            bool showPrev = skip > 0;
            bool showNext = (skip + _constants.NbrResultsPerPage) < totalHits;

            var html = new StringBuilder();

            html.Append("<p>");

            if (showPrev)
            {
                html.Append(PrevNextLinkHtml("prev", "Previous", skip - _constants.NbrResultsPerPage, searchTerm));
            }

            if (showPrev && showNext)
            {
                html.Append(" | ");
            }

            if (showNext)
            {
                html.Append(PrevNextLinkHtml("next", "Next", skip + _constants.NbrResultsPerPage, searchTerm));
            }

            html.Append("</p>");

            return html.ToString();
        }

        private string PrevNextLinkHtml(string id, string linkName, int skip, string searchTerm)
        {
            string html = string.Format(@"<a id=""{0}"" href=""/SearchResults?q={3}&skip={2}"">{1}</a>",
                id, linkName, skip, HttpUtility.UrlEncode(searchTerm));

            return html;
        }
    }
}
