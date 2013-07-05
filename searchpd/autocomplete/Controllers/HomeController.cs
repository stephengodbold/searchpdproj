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
    }
}

