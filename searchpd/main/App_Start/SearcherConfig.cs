using Autofac;
using main.Models;
using searchpd.Search;

namespace main.App_Start
{
    public static class SearcherConfig
    {
        /// <summary>
        /// Loads any caches.
        /// </summary>
        /// <param name="container">
        /// The Autofac container. Needed to resolve the searcher.
        /// </param>
        public static void LoadCaches(IContainer container)
        {
            // Creating these scopes on the fly is not nice.
            // But we do want to load the suggestions caches at application startup, so doesn't seem to be much choice.

            using (var scope = container.BeginLifetimeScope())
            {
                var constants = scope.Resolve<IConstants>();

                var searcher = scope.Resolve<IProductSearcher>();
                searcher.LoadProductStore(constants.LucenePath, constants.ProductCodeBoost);
            }
        }
    }
}

