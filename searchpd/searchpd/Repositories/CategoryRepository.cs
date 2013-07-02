using System.Collections.Generic;
using System.Linq;
using searchpd.Models;

namespace searchpd.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryHierarchy> GetAllHierarchies();
        Category GetCategoryById(int categoryId);
    }

    public class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Gets all category hierarchies from the database, sorted by parent name, than by category name.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoryHierarchy> GetAllHierarchies()
        {
            using (var context = new searchpdEntities())
            {
                // This will not return the top most category WEBONLINE, because its 0 ParentID doesn't join with any other category.
                IEnumerable<CategoryHierarchy> hierarchies = 
                    (from c in context.Categories
                    join p in context.Categories on c.ParentID equals p.CategoryID
                    select new CategoryHierarchy
                        {
                            Category = c, 
                            Parent = ((p.ParentID == 0) ? null : p) // see description of CategoryHierachy
                        })
                        .OrderBy(c => (c.Parent == null) ? "" : c.Parent.Name)
                        .ThenBy(c=>c.Category.Name)
                        .ToList(); 

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

