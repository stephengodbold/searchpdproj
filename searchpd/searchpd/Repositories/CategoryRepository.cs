using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using searchpd.Models;

namespace searchpd.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryHierarchy> GetAllHierarchies();
    }

    public class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Gets all category hierarchies from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoryHierarchy> GetAllHierarchies()
        {
            IEnumerable<CategoryHierarchy> hierarchies = null;

            using (var context = new searchpdEntities())
            {
                // This will not return the top most category WEBONLINE, because its 0 ParentID doesn't join with any other category.
                hierarchies = 
                    (from c in context.Categories
                    join p in context.Categories on c.ParentID equals p.CategoryID
                    select new CategoryHierarchy
                        {
                            Category = c, 
                            Parent = ((p.ParentID == 0) ? null : p) // see description of CategoryHierachy
                        }).ToList(); 

                //TODO: Optimise this by using a left join instead
            }

            return hierarchies;
        }
    }
}

