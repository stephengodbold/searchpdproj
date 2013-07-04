using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core;
using searchpd.Search;

namespace autocomplete.App_Start
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
                var searcher = scope.Resolve<ISearcher>();
                searcher.EnsureSuggestions();
            }
        }
    }
}

