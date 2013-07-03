using System.Collections.Generic;
using System.Linq;
using searchpd.Models;

namespace searchpd.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<CategorySuggestion> GetAllHierarchies();
        Category GetCategoryById(int categoryId);
    }

    public class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Gets all category hierarchies from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategorySuggestion> GetAllHierarchies()
        {
            using (var context = new searchpdEntities())
            {
                // This will not return the top most category WEBONLINE, because its 0 ParentID doesn't join with any other category.
                IEnumerable<CategorySuggestion> hierarchies = 
                    (from c in context.Categories
                    join p in context.Categories on c.ParentID equals p.CategoryID
                    select CategorySuggestion.Create(c.Name, c.CategoryID, (p.ParentID == 0) ? null : p.Name, p.CategoryID)).ToList(); 

                //TODO: Optimise this by using a left join instead

                return hierarchies;
            }
        }


        /// <summary>
        /// Returns a category given its id, or null if the id does not match a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId)
        {
            using (var context = new searchpdEntities())
            {
                Category category = context.Categories.Find(categoryId);

                return category;
            }
        }
    }
}

