using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using searchpd.Models;
using searchpd.Repositories;
using searchpd.Search;
//############################
namespace main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearcher _searcher = null;

        public HomeController(ISearcher searcher)
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

