using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using searchpd.Search;
using System.Configuration;

// Install this program on the main IIS server.
// Update its app.config file with the path to the Lucene directory, etc.
//
// This program refreshes the Product Search Lucene index.
namespace mainRefresh
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(ProductSearcher).Assembly);
            var container = builder.Build();

            string lucenePath = ConfigurationManager.AppSettings["LucenePath"];
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string absoluteLucenePath = Path.Combine(rootPath, lucenePath);

            float productCodeBoost = float.Parse(ConfigurationManager.AppSettings["ProductCodeBoost"]);
            float minSimilarity = float.Parse(ConfigurationManager.AppSettings["MinSimilarity"]);

            using (var scope = container.BeginLifetimeScope())
            {
                var searcher = scope.Resolve<IProductSearcher>();
                searcher.LoadProductStore(absoluteLucenePath, productCodeBoost, minSimilarity, false);
            }

            Console.WriteLine("Product search results updated");
        }
    }
}
