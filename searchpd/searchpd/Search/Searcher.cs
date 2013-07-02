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

        public IEnumerable<CategoryHierarchy> FindCategoryHierarchiesBySubstring(string subString)
        {
            // TODO: EXTREMELY INEFFICIENT IMPLEMENTATION
            // Will be replace by Lucene based code.
            LoadCategoryHierarchies();
            string subStringLC = subString.ToLower();
            var selectedHierarchies = _allHierarchies.Where(s => s.Category.Name.ToLower().Contains(subStringLC));

            return selectedHierarchies;
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

