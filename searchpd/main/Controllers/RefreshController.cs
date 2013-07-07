using System.Web.Mvc;
using main.Models;
using searchpd.Search;

namespace main.Controllers
{
    /// <summary>
    /// You could use this controller to initiate re-indexing from the admin site.
    /// In that case, you'd need to add an authorize attribute here to only authorised personnel can
    /// start a re-index.
    /// 
    /// Otherwise, this controller is for demo purposes only and should not go live.
    /// </summary>
    public class RefreshController : Controller
    {
        private readonly IProductSearcher _searcher;
        private readonly IAutoupdateRefresher _autoupdateRefresher;
        private readonly IConstants _constants;

        public RefreshController(IProductSearcher searcher, IAutoupdateRefresher autoupdateRefresher, IConstants constants)
        {
            _searcher = searcher;
            _autoupdateRefresher = autoupdateRefresher;
            _constants = constants;
        }

        /// <summary>
        /// Loads a new Lucene index. 
        /// 
        /// In operational use, you would run this action after database changes.
        /// While this action is running (can take a while), other requests can still access the existing index.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SearchResults()
        {
            _searcher.LoadProductStore(_constants.AbsoluteLucenePath, _constants.ProductCodeBoost, _constants.MinSimilarity, false);

            return "Product search results updated";
        }

        /// <summary>
        /// Sends request to the autoupdate refresher to refresh its suggestions from the database.
        /// 
        /// In operational use, you would run this action after database changes.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Suggestions()
        {
            string refreshResponse = _autoupdateRefresher.RefreshAutoupdate(
                _constants.AutocompleteRefreshUrl, _constants.AutocompleteRefreshPassword);

            return refreshResponse;
        }
    }
}
