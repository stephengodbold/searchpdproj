using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web;
using searchpd.Models;
using searchpd.Repositories;
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

        // GET api/search?q=substring
        public string GetBySubstring(string q)
        {
            string subString = q;

            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring(subString);

            var html = new StringBuilder();

            foreach (ISuggestion suggestion in suggestions)
            {
                html.AppendFormat(@"<div class=""suggestion"">");
                html.AppendFormat(suggestion.ToHtml(subString));
                html.AppendFormat(@"</div>");
            }

            return html.ToString();
        }
    }
}

