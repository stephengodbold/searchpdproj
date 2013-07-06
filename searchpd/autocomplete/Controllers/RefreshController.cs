using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using autocomplete.Models;
using searchpd.Models;
using searchpd.Search;

namespace autocomplete.Controllers
{
    public class RefreshController : Controller
    {
        private readonly ISuggestionSearcher _searcher;
        private readonly IConstants _constants;

        public RefreshController(ISuggestionSearcher searcher, IConstants constants)
        {
            _searcher = searcher;
            _constants = constants;
        }

        // POST /refresh/suggestions
        // Reloads the Lucene index with suggestions. 
        [System.Web.Http.HttpPost]
        public string Suggestions(string password)
        {
            if (password != _constants.AutocompleteRefreshPassword)
            {
                return "Not authorised";
            }

            _searcher.LoadSuggestionsStore(_constants.AbsoluteLucenePath, false);
            return "Suggestions updated";
        }
    }
}
