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
        void LoadCategoryHierarchies();
    }

    public class Searcher : ISearcher
    {
        private static IEnumerable<CategorySuggestion> _allHierarchies = null;

        private readonly ICategoryRepository _categoryRepository = null;

        public Searcher(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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

            // TODO: EXTREMELY INEFFICIENT IMPLEMENTATION
            // Will be replace by Lucene based code.
            LoadCategoryHierarchies();
            string subStringLC = subString.ToLower();
            var selectedHierarchies = _allHierarchies.Where(s => s.CategoryName.ToLower().Contains(subStringLC));

            var sortedHierarchies = selectedHierarchies
                .OrderBy(c => (c.HasParent) ? c.ParentName : "")
                .ThenBy(c => c.CategoryName);

            return sortedHierarchies;
        }

        public void LoadCategoryHierarchies()
        {
            if (_allHierarchies == null)
            {
                _allHierarchies = _categoryRepository.GetAllHierarchies();
            }
        }
    }
}

