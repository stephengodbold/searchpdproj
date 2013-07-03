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
        void LoadSuggestions();
    }

    public class Searcher : ISearcher
    {
        private static IEnumerable<CategorySuggestion> _allCategorySuggestions = null;
        private static IEnumerable<ProductSuggestion> _allProductSuggestions = null;

        private readonly ICategoryRepository _categoryRepository = null;
        private readonly IProductRepository _productRepository = null;

        public Searcher(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
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

            LoadSuggestions();
            string subStringLC = subString.ToLower();

            var selectedHierarchies = _allCategorySuggestions.Where(s => s.CategoryName.ToLower().Contains(subStringLC));

            var sortedCategorySuggestions = selectedHierarchies
                .OrderBy(c => (c.HasParent) ? c.ParentName : "")
                .ThenBy(c => c.CategoryName);

            var selectedProductSuggestions = _allProductSuggestions
                .Where(p => p.ProductCode.ToLower().Contains(subStringLC));

            var sortedProductSuggestions =
                selectedProductSuggestions.OrderBy(p => p.ProductCode);

            var finalSuggestions = sortedCategorySuggestions.Union<ISuggestion>(sortedProductSuggestions).ToList();

            return finalSuggestions;
        }

        public void LoadSuggestions()
        {
            if (_allCategorySuggestions == null)
            {
                _allCategorySuggestions = _categoryRepository.GetAllSuggestions();
            }

            if (_allProductSuggestions == null)
            {
                _allProductSuggestions = _productRepository.GetAllSuggestions();
            }
        }
    }
}

