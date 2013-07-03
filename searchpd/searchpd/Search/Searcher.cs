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
        IEnumerable<CategoryHierarchy> FindCategoryHierarchiesBySubstring(string subString);
        void LoadCategoryHierarchies();
    }

    public class Searcher : ISearcher
    {
        private static IEnumerable<CategoryHierarchy> _allHierarchies = null;

        private readonly ICategoryRepository _categoryRepository = null;

        public Searcher(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Returns a collection of all CategoryHierarchies that match the given sub string.
        /// If the sub string is null or empty, or doesn't match any categories, than an empty list is returned.
        /// </summary>
        /// <param name="subString"></param>
        /// <returns></returns>
        public IEnumerable<CategoryHierarchy> FindCategoryHierarchiesBySubstring(string subString)
        {
            if (string.IsNullOrEmpty(subString))
            {
                return new List<CategoryHierarchy>();
            }

            // TODO: EXTREMELY INEFFICIENT IMPLEMENTATION
            // Will be replace by Lucene based code.
            LoadCategoryHierarchies();
            string subStringLC = subString.ToLower();
            var selectedHierarchies = _allHierarchies.Where(s => s.Category.Name.ToLower().Contains(subStringLC));

            var sortedHierarchies = selectedHierarchies
                .OrderBy(c => (c.Parent == null) ? "" : c.Parent.Name)
                .ThenBy(c => c.Category.Name);

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

