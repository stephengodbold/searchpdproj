using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using searchpd.Models;
using searchpd.Search;

namespace autocomplete.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionSearcher _searcher;

        public SuggestionsController(ISuggestionSearcher searcher)
        {
            _searcher = searcher;
        }

        //
        // GET: /Suggestions/

        public string Index(string q, string callback)
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

            string finalHtml = html.ToString();
            string jsonpResponse = string.Format("{0}(\'{1}\')", callback, HttpUtility.JavaScriptStringEncode(finalHtml));
            return jsonpResponse;
        }

    }
}
