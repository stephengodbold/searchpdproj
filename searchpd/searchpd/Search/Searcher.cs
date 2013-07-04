using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using searchpd.Models;
using searchpd.Repositories;

namespace searchpd.Search
{
    public interface ISearcher
    {
        IEnumerable<ISuggestion> FindSuggestionsBySubstring(string subString);
        void EnsureSuggestions();
        void RefreshSuggestions();
    }

    public class Searcher : ISearcher
    {
        private const string CacheKeyCategorySuggestions = "__Searcher_CategorySuggestions";
        private const string CacheKeyProductSuggestions = "__Searcher_ProductSuggestions";
        private static readonly object CacheLock = new object();

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly HttpContextBase _httpContextBase;

        public Searcher(ICategoryRepository categoryRepository, IProductRepository productRepository,
            HttpContextBase httpContextBase)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _httpContextBase = httpContextBase;
        }

        /// <summary>
        /// Returns a collection of all suggestions that match the given sub string.
        /// If the sub string is null or empty, or doesn't match any categories, than an empty list is returned.
        /// The suggestions will be returned in the correct order.
        /// </summary>
        /// <param name="subString"></param>
        /// <returns></returns>
        public IEnumerable<ISuggestion> FindSuggestionsBySubstring(string subString)
        {
            if (string.IsNullOrEmpty(subString))
            {
                return new List<CategorySuggestion>();
            }

            // TODO: Not very efficient implementation. To be replaced by something better if it turns out too slow.
            // A simple way to speed this up would be by caching search results, with relatively low priority (so they
            // get scavenged first). This would probably work because the site probably has a relative small number of 
            // tradies doing the same searches repeatedly.

            string subStringLc = subString.ToLower();

            var selectedHierarchies = AllCategorySuggestions().Where(s => s.CategoryName.ToLower().Contains(subStringLc));

            var sortedCategorySuggestions = selectedHierarchies
                .OrderBy(c => (c.HasParent) ? c.ParentName : "")
                .ThenBy(c => c.CategoryName);

            var selectedProductSuggestions = AllProductSuggestions()
                .Where(p => p.ProductCode.ToLower().Contains(subStringLc));

            var sortedProductSuggestions =
                selectedProductSuggestions.OrderBy(p => p.ProductCode);

            var finalSuggestions = sortedCategorySuggestions.Union<ISuggestion>(sortedProductSuggestions).ToList();

            return finalSuggestions;
        }

        /// <summary>
        /// Stores the suggestions in cache if they are not already there.
        /// </summary>
        public void EnsureSuggestions()
        {
            AllCategorySuggestions();
            AllProductSuggestions();
        }

        /// <summary>
        /// Refreshes the suggestion caches from the database.
        /// </summary>
        public void RefreshSuggestions()
        {
            _httpContextBase.Cache.Remove(CacheKeyCategorySuggestions);
            _httpContextBase.Cache.Remove(CacheKeyProductSuggestions);
            EnsureSuggestions();
        }

        private IEnumerable<CategorySuggestion> AllCategorySuggestions()
        {
            return RetrieveFromCache(CacheKeyCategorySuggestions, _categoryRepository.GetAllSuggestions);
        }

        private IEnumerable<ProductSuggestion> AllProductSuggestions()
        {
            return RetrieveFromCache(CacheKeyProductSuggestions, _productRepository.GetAllSuggestions);
        }

        private T RetrieveFromCache<T>(string cacheKey, Func<T> generateData) where T : class
        {
            var data = (T)_httpContextBase.Cache[cacheKey];
            if (data == null)
            {
                lock (CacheLock)
                {
                    // Ensure that the data was not loaded by a concurrent thread 
                    // while waiting for lock.
                    data = (T)_httpContextBase.Cache[cacheKey];
                    if (data == null)
                    {
                        data = generateData();
                        _httpContextBase.Cache[cacheKey] = data;
                    }
                }
            }

            return data;
        }
    }
}

