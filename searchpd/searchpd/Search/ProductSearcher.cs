using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using searchpd.Models;
using searchpd.Repositories;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.Web.Caching;

namespace searchpd.Search
{
    public interface IProductSearcher
    {
        IEnumerable<ProductSearchResult> FindProductsBySearchTerm(string searchTerm);
        void LoadProductStore(string lucenePath, float productCodeBoost);
        void RefreshProductStore();
    }

    public class ProductSearcher : IProductSearcher
    {
        private const string CacheKeySearcher = "__ProductSearcher_Searcher";
        private const Version LuceneVersion = Version.LUCENE_30;

        // This object is a singleton, so should be able to store things safely in a property.
        private IndexSearcher _searcher;

        private readonly IProductRepository _productRepository;

        public ProductSearcher(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Finds all products that match the given search term.
        /// 
        /// Returns the products in order of relevancy, most relevant first.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IEnumerable<ProductSearchResult> FindProductsBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<ProductSearchResult>();
            }

            // The StandardAnalyzer stores everything in lower case. So need to convert search term to lower case as well.
            string searchTermLc = searchTerm.ToLower();

            var booleanQuery = new BooleanQuery();
            Query query1 = new WildcardQuery(new Term("ProductCode", "*" + searchTermLc + "*"));
            Query query2 = new TermQuery(new Term("ProductDescription", searchTermLc));

            // Making both fields SHOULD means only retrieve documents where at least 1 field matches
            booleanQuery.Add(query1, Occur.SHOULD);
            booleanQuery.Add(query2, Occur.SHOULD);

            // Actual search
            TopDocs hits = _searcher.Search(booleanQuery, _searcher.MaxDoc);

            // Return results. ScoreDocs is already sorted by relevancy desc.
            var sortedResults = hits
                .ScoreDocs
                .Select(d =>
                    {
                        var doc = _searcher.Doc(d.Doc);
                        return new ProductSearchResult
                            {
                                ProductId = int.Parse(doc.Get("ProductId")),
                                ProductCode = doc.Get("ProductCode"),
                                ProductDescription = doc.Get("ProductDescription")
                            };
                    });

            return sortedResults;
        }

        /// <summary>
        /// Ensures that the products have been loaded in the Lucene index 
        /// </summary>
        /// <param name="lucenePath">
        /// Path relative to the root of the current web app where the Lucene files get stored.
        /// </param>
        /// <param name="productCodeBoost">
        /// Weigthing to give to product code relative to product description.
        /// If this is 5, than product code has 5 times the weight of product description.
        /// </param>
        public void LoadProductStore(string lucenePath, float productCodeBoost)
        {
            // We've got dependencies here on the run time. Need to remove these before we can unit test
            // this method.
            string rootPath = HttpRuntime.AppDomainAppPath;
            string absoluteLucenePath = Path.Combine(rootPath, lucenePath);

            var luceneDir = new DirectoryInfo(absoluteLucenePath);
            if (!luceneDir.Exists)
            {
                luceneDir.Create();
                luceneDir.Refresh();
            }

            var directory = new SimpleFSDirectory(luceneDir);
            
            // Create an analyzer that uses standard analyzer for all fields, but keyword analyzer for ProductCode
            // (because we want to regard product codes as 1 word).
            var perFieldAnalyzerWrapper = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion));
            perFieldAnalyzerWrapper.AddAnalyzer("ProductCode", new KeywordAnalyzer());
            Analyzer analyzer = perFieldAnalyzerWrapper;

            var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            _searcher = new IndexSearcher(directory, true);

            // -----------
            // Store products into Lucene

            IEnumerable<ProductSearchResult> results = _productRepository.GetAllProductSearchResults();

            foreach (var result in results)
            {
                var doc = new Document();
                doc.Add(new Field("ProductId", result.ProductId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NO));

                var productCodeField = new Field("ProductCode", result.ProductCode, Field.Store.YES,
                                                 Field.Index.ANALYZED);
                productCodeField.Boost = productCodeBoost;
                doc.Add(productCodeField);

                doc.Add(new Field("ProductDescription", result.ProductDescription, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }

            writer.Commit();
        }

        public void RefreshProductStore()
        {
            
        }
    }
}