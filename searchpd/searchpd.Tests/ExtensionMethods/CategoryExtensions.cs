using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using searchpd.Models;

namespace searchpd.Tests.ExtensionMethods
{
    public static class CategoryExtensions
    {
        public static bool EqualTo(this Category category1, Category category2)
        {
            if (category1 == category2)
            {
                return true;
            }

            if ((category1 == null) || (category2 == null))
            {
                return false;
            }

            // Not checking on equality of ParentID, because the application doesn't use that field in higher levels
            // and does not necessarily keep it correct. Eg. not storing this in search infracture.
            // There is a case here for introducing a separate Category class that only has the CategoryID and Name.
            return ((category1.Name == category2.Name) && (category1.CategoryID == category2.CategoryID));
        }

        public static bool EqualTo(this CategoryHierarchy categoryHierarchy1, CategoryHierarchy categoryHierarchy2)
        {
            if (categoryHierarchy1 == categoryHierarchy2)
            {
                return true;
            }

            if ((categoryHierarchy1 == null) || (categoryHierarchy2 == null))
            {
                return false;
            }

            return ((categoryHierarchy1.Category.EqualTo(categoryHierarchy2.Category)) && (categoryHierarchy1.Parent.EqualTo(categoryHierarchy2.Parent)));
        }

        public static bool EqualTo(this IEnumerable<CategoryHierarchy> categoryHierarchies1, IEnumerable<CategoryHierarchy> categoryHierarchies2)
        {
            if (categoryHierarchies1 == categoryHierarchies2)
            {
                return true;
            }

            if ((categoryHierarchies1 == null) || (categoryHierarchies2 == null))
            {
                return false;
            }

            int nbrHierarchies = categoryHierarchies1.Count();
            if (nbrHierarchies != categoryHierarchies2.Count())
            {
                return false;
            }

            for (int i = 0; i < nbrHierarchies; i++)
            {
                if (!categoryHierarchies1.ElementAt(i).EqualTo(categoryHierarchies2.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
