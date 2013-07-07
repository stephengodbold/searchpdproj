using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using searchpd.Models;
using searchpd.Repositories;

namespace searchpd.Search
{
    public interface ISuggestionSearcher
    {
        IEnumerable<ISuggestion> FindSuggestionsBySubstring(string subString);
        void LoadSuggestionsStore(string absoluteLucenePath, bool onlyIfNotExists);
    }

    public class SuggestionSearcher : LuceneSearcher, ISuggestionSearcher
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        private const string CategorySuggestionCode = "c";
        private const string ProductSuggestionCode = "p";

        public SuggestionSearcher(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Returns a collection of all suggestions that match the given searchTerm.
        /// A match means search term appears anywhere in the product code or category name.
        /// 
        /// If the searchTerm is null or empty, or doesn't match any categories, than an empty list is returned.
        /// The suggestions will be returned in the correct order.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IEnumerable<ISuggestion> FindSuggestionsBySubstring(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<CategorySuggestion>();
            }

            // The product codes and category names have been stored in upper case. So need to convert search term to upper case as well.
            string searchTermUc = LuceneEscape(searchTerm.ToUpper());

            Query query1 = new WildcardQuery(new Term("UcName", "*" + searchTermUc + "*"));

            // Get the searcher. Access _searcher only once while doing a search. Another request running 
            // LoadProductStore could change this property. By accessing once, you are sure your searcher stays the same.
            var searcher = _searcher;

            // Actual search

            TopDocs hits = searcher.Search(query1, searcher.MaxDoc);

            // Return results. ScoreDocs is sorted by relevancy, but we want alphabetic sorting, and category suggestions first.
            IEnumerable<ISuggestion> sortedResults = hits.ScoreDocs
                .Select(d =>
                {
                    var doc = searcher.Doc(d.Doc);
                    return DocToSuggestion(doc);
                })
                .OrderBy(s=>(s is CategorySuggestion ? 0 : 1))
                .ThenBy(s=>s.SortedName);

            return sortedResults;
        }

        /// <summary>
        /// Ensures that the suggestions have been loaded in the Lucene index 
        /// 
        /// SIDE EFFECT
        /// This method sets property _searcher in the base class
        /// </summary>
        /// <param name="absoluteLucenePath">
        /// Absolute path of the directory where the Lucene files get stored.
        /// </param>
        /// <param name="onlyIfNotExists">
        /// If true, the index will only be created if there is no index at all (that is, no Lucene directory).
        /// If false, this will always create a new index.
        /// </param>
        public void LoadSuggestionsStore(string absoluteLucenePath, bool onlyIfNotExists)
        {
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
            Analyzer analyzer = new KeywordAnalyzer();

            // -----------
            // Store products into Lucene.
            // This will create a new index. Other requests will still be able to read the existing index.

            // Create writer that will overwrite the existing index
            using (var writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IEnumerable<ProductSuggestion> productSuggestions = _productRepository.GetAllSuggestions();

                foreach (var productSuggestion in productSuggestions)
                {
                    // Storing all names in upper case, so we can do case insensitive search easily

                    var doc = new Document();
                    doc.Add(new Field("Object", productSuggestion.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("UcName", productSuggestion.ProductCode.ToUpper(), Field.Store.NO, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("SuggestionType", ProductSuggestionCode , Field.Store.YES, Field.Index.NO));
                    writer.AddDocument(doc);
                }

                IEnumerable<CategorySuggestion> categorySuggestions = _categoryRepository.GetAllSuggestions();

                foreach (var categorySuggestion in categorySuggestions)
                {
                    var doc = new Document();
                    doc.Add(new Field("Object", categorySuggestion.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("UcName", categorySuggestion.CategoryName.ToUpper(), Field.Store.NO, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("SuggestionType", CategorySuggestionCode, Field.Store.YES, Field.Index.NO));
                    writer.AddDocument(doc);
                }
            }
        }

        private ISuggestion DocToSuggestion(Document doc)
        {
            string suggestionType = doc.Get("SuggestionType");
            string serialisedObject = doc.Get("Object");

            switch (suggestionType)
            {
                case ProductSuggestionCode:
                    return ProductSuggestion.Parse(serialisedObject);

                case CategorySuggestionCode:
                    return CategorySuggestion.Parse(serialisedObject);

                default:
                    throw new Exception(string.Format("Unknown suggestion type: {0}", suggestionType));
            }
        }
    }
}

