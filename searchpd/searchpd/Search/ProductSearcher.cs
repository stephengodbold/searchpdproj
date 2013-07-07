using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using searchpd.Models;
using searchpd.Repositories;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using searchpd.Search.Analyzers;

namespace searchpd.Search
{
    public interface IProductSearcher
    {
        IEnumerable<ProductSearchResult> FindProductsBySearchTerm(string searchTerm, int skip, int take, out int totalHits);
        void LoadProductStore(string absoluteLucenePath, float productCodeBoost, float minSimilarity, bool onlyIfNotExists);
    }

    public class ProductSearcher : LuceneSearcher, IProductSearcher
    {
        private readonly IProductRepository _productRepository;
        private float _productCodeBoost = 1.0f;
        private float _minSimilarity = 0.5f;

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
        /// <param name="skip">
        /// The first "skip" results will be skipped (not returned).
        /// </param>
        /// <param name="take">
        /// The first take results after the skip results will be returned, or less if fewer available.
        /// </param>
        /// <param name="totalHits">
        /// Total number of hits produced by the query.
        /// </param>
        /// <returns></returns>
        public IEnumerable<ProductSearchResult> FindProductsBySearchTerm(string searchTerm, int skip, int take, out int totalHits)
        {
            totalHits = 0;

            if (string.IsNullOrWhiteSpace(searchTerm) || (take < 1))
            {
                return new List<ProductSearchResult>();
            }

            if (skip < 0)
            {
                throw new ArgumentException(string.Format("skip = {0}. Should be >= 0", skip));
            }

            // All product codes and descriptions are stored in upper case. So need to convert search term to upper case as well.
            string searchTermUc = LuceneEscape(searchTerm.ToUpper());

            Analyzer analyzer = new UpperCaseLetterOrDigitAnalyzer();

            // FuzzyLikeThisQuery prepares a list of words that fuzzily match the given search term
            // (added with AddTerms), and then only retains the top maxNumTerms by closeness.
            // This allows you to ignore words that are "too" fuzzy.
            //
            // Do not set maxNumTerms too high, or you get an out of memory error.
            const int maxNumTerms = 100;
            var query1 = new FuzzyLikeThisQuery(maxNumTerms, analyzer);
            query1.AddTerms(searchTermUc, "ProductDescription", _minSimilarity, 0);
            
            Query query2 = new WildcardQuery(new Term("ProductCode", "*" + searchTermUc + "*"));
            query2.Boost = _productCodeBoost;

            // Making both fields SHOULD means only retrieve documents where at least 1 field matches
            var booleanQuery = new BooleanQuery();
            booleanQuery.Add(query1, Occur.SHOULD);
            booleanQuery.Add(query2, Occur.SHOULD);

            // Get the searcher. Access _searcher only once while doing a search. Another request running 
            // LoadProductStore could change this property. By accessing once, you are sure your searcher stays the same
            // during the search.
            var searcher = _searcher;

            // Actual search

            int maxWanted = Math.Min(skip + take, searcher.MaxDoc);
            TopDocs hits = searcher.Search(booleanQuery, maxWanted);

            totalHits = hits.TotalHits;

            // Return results. ScoreDocs is already sorted by relevancy desc.
            var sortedResults = hits.ScoreDocs
                .Skip(skip)
                .Take(maxWanted - skip)
                .Select(d =>
                    {
                        var doc = searcher.Doc(d.Doc);
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
        /// 
        /// SIDE EFFECT
        /// This method sets property _searcher
        /// </summary>
        /// <param name="absoluteLucenePath">
        /// Absolute path of the directory where the Lucene files get stored.
        /// </param>
        /// <param name="productCodeBoost">
        /// Weigthing to give to product code relative to product description.
        /// If this is 5, than product code has 5 times the weight of product description.
        /// </param>
        /// <param name="minSimilarity">
        /// Minimum similarity used when fuzzy matching against product descriptions.
        /// </param>
        /// <param name="onlyIfNotExists">
        /// If true, the index will only be created if there is no index at all (that is, no Lucene directory).
        /// If false, this will always create a new index.
        /// </param>
        public void LoadProductStore(string absoluteLucenePath, float productCodeBoost, float minSimilarity, bool onlyIfNotExists)
        {
            // Setting boost on fields doesn't work when that field is used in a wildcard search,
            // as is done for product boost. So store the boost and apply it when doing the query.

            _productCodeBoost = productCodeBoost;
            _minSimilarity = minSimilarity;

            LoadStore(absoluteLucenePath, onlyIfNotExists, LoadLuceneIndex);
        }

        /// <summary>
        /// Loads the data into the Lucene index
        /// </summary>
        /// <param name="directory">
        /// Directory where the index is located.
        /// </param>
        private void LoadLuceneIndex(SimpleFSDirectory directory)
        {
            // Create an analyzer that uses UpperCaseLetterOrDigitAnalyzer for all fields, but UpperCaseKeywordAnalyzer for ProductCode
            // (because we want to regard product codes as 1 word).

            var analyzer = new PerFieldAnalyzerWrapper(new UpperCaseLetterOrDigitAnalyzer());
            analyzer.AddAnalyzer("ProductCode", new UpperCaseKeywordAnalyzer());

            // -----------
            // Store products into Lucene.
            // This will create a new index. Other requests will still be able to read the existing index.

            // Create writer that will overwrite the existing index
            using (var writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IEnumerable<ProductSearchResult> results = _productRepository.GetAllProductSearchResults();

                foreach (var result in results)
                {
                    var doc = new Document();
                    doc.Add(new Field("ProductId", result.ProductId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NO));

                    // Store field in index so it can be searched, but don't analyse it - just store as is.
                    var productCodeField = new Field("ProductCode", result.ProductCode, Field.Store.YES, Field.Index.ANALYZED);
                    doc.Add(productCodeField);

                    doc.Add(new Field("ProductDescription", result.ProductDescription, Field.Store.YES, Field.Index.ANALYZED));

                    writer.AddDocument(doc);
                }
            }
        }
    }
}

