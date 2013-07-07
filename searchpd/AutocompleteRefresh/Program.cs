using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using searchpd.Search;
using System.Configuration;

// Install this program on the IIS server running autocomplete.
// Update its app.config file with the path to the Lucene directory, etc.
//
// This program refreshes the Suggestions Lucene index.
namespace AutocompleteRefresh
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(SuggestionSearcher).Assembly);
            var container = builder.Build();

            string lucenePath = ConfigurationManager.AppSettings["LucenePath"];
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string absoluteLucenePath = Path.Combine(rootPath, lucenePath);

            using (var scope = container.BeginLifetimeScope())
            {
                var searcher = scope.Resolve<ISuggestionSearcher>();
                searcher.LoadSuggestionsStore(absoluteLucenePath, false);
            }

            Console.WriteLine("Suggestions updated");
        }
    }
}
