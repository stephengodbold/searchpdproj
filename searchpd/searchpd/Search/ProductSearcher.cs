using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using searchpd.Models;
using searchpd.Repositories;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace searchpd.Search
{
    public interface IProductSearcher
    {
        IEnumerable<ProductSearchResult> FindProductsBySearchTerm(string searchTerm, int skip, int take, out int totalHits);
        void LoadProductStore(string absoluteLucenePath, float productCodeBoost, bool onlyIfNotExists);
    }

    public class ProductSearcher : LuceneSearcher, IProductSearcher
    {
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

            // Get the searcher. Access _searcher only once while doing a search. Another request running 
            // LoadProductStore could change this property. By accessing once, you are sure your searcher stays the same.
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
                                ProductCode = doc.Get("ProductCode").ToUpper(),//TODO:######## dirty hack, to be removed when lower case filter introduced for product code
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
        /// <param name="onlyIfNotExists">
        /// If true, the index will only be created if there is no index at all (that is, no Lucene directory).
        /// If false, this will always create a new index.
        /// </param>
        public void LoadProductStore(string absoluteLucenePath, float productCodeBoost, bool onlyIfNotExists)
        {
            LoadStore(absoluteLucenePath, onlyIfNotExists,
                      (directory) => { LoadLuceneIndex(directory, productCodeBoost); });
        }

        /// <summary>
        /// Loads the data into the Lucene index
        /// </summary>
        /// <param name="directory">
        /// Directory where the index is located.
        /// </param>
        /// <param name="productCodeBoost">
        /// Weigthing to give to product code relative to product description.
        /// If this is 5, than product code has 5 times the weight of product description.
        /// </param>
        private void LoadLuceneIndex(SimpleFSDirectory directory, float productCodeBoost)
        {
            // Create an analyzer that uses standard analyzer for all fields, but keyword analyzer for ProductCode
            // (because we want to regard product codes as 1 word).
            var perFieldAnalyzerWrapper = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion));
            perFieldAnalyzerWrapper.AddAnalyzer("ProductCode", new KeywordAnalyzer());
            Analyzer analyzer = perFieldAnalyzerWrapper;

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

                    //  #####              var productCodeField = new Field("ProductCode", result.ProductCode, Field.Store.YES,
                    var productCodeField = new Field("ProductCode", result.ProductCode.ToLower(), Field.Store.YES,
                                                     Field.Index.ANALYZED);
                    productCodeField.Boost = productCodeBoost;
                    doc.Add(productCodeField);

                    doc.Add(new Field("ProductDescription", result.ProductDescription, Field.Store.YES, Field.Index.ANALYZED));

                    writer.AddDocument(doc);
                }
            }
        }
    }
}

