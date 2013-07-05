using System.Web.Mvc;
using searchpd.Search;

namespace main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISuggestionSearcher _searcher;

        public HomeController(ISuggestionSearcher searcher)
        {
            _searcher = searcher;
        }

        // This form will only be posted to by 
        // /home/admin 
        // in the main site.

        // POST /home/refresh
        // Reloads the suggestions cache
        [System.Web.Http.HttpPost]
        public ActionResult Refresh()
        {
            _searcher.RefreshSuggestions();
            return View();
        }
    }
}

