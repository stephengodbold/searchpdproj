using System.Collections.Generic;
using System.Linq;
using searchpd.Models;

namespace searchpd.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<CategorySuggestion> GetAllSuggestions();
        Category GetCategoryById(int categoryId);
    }

    public class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Gets all category suggestions from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategorySuggestion> GetAllSuggestions()
        {
            using (var context = new searchpdEntities())
            {
                // This will not return the top most category WEBONLINE, because its 0 ParentID doesn't join with any other category.
                IEnumerable<CategorySuggestion> suggestions =
                    context.Categories.Join(
                        context.Categories,
                        c => c.ParentID,
                        p => p.CategoryID,
                        (c, p) => new CategorySuggestion
                            {
                                CategoryName = c.Name,
                                CategoryId = c.CategoryID,
                                ParentName = (p.ParentID == 0) ? null : p.Name,
                                ParentId = p.ParentID
                            }).ToList();

                //TODO: Optimise this by using a left join instead

                return suggestions;
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

