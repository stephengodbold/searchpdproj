using System.Globalization;
using System.Text;

namespace searchpd.Models
{
    /// <summary>
    /// Represents a category suggestion - a category and its parent.
    /// 
    /// If the parent of a category in the database is the top most category WEBONLINE with ParentID equals 0, the ParentName property 
    /// in this record will be null.
    /// 
    /// Storing the parent name and id, rather than a reference, is a bit wasteful.
    /// However, there are only about 3000 categories, and category names are on average 15 chars long - so we're wasting about 60KB which is not much.
    /// </summary>
    public class CategorySuggestion : DisplayObject, ISuggestion
    {
        private const string CategoryPageUrl = "/Home/Categories/{0}";

        public string CategoryName { get; set; }
        public int CategoryId { get; set; } // Be sure never to update this after object creation, it is used as the hash code.

        // "Parent" refers to the parent category of this category.
        // If there is no parent, ParentId will be 0
        public string ParentName { get; set; }
        public int ParentId { get; set; }

        public bool HasParent { get { return (ParentId > 0); } }

        public string SortedName { get { return ParentName + " " + CategoryName; } }

        public CategorySuggestion()
        {
        }

        public CategorySuggestion(string categoryName, int categoryId, string parentName, int parentId)
        {
            CategoryName = categoryName;
            CategoryId = categoryId;
            ParentName = parentName;
            ParentId = parentId;
        }

        /// <summary>
        /// Returns the html representation of a category suggestion
        /// </summary>
        /// <param name="subString">
        /// Sub string to highlight in the html. Pass null to not hightlight anything.
        /// </param>
        /// <returns></returns>
        public string ToHtml(string subString)
        {
            var html = new StringBuilder();
            html.AppendFormat(HighlightedAnchor(CategoryName, subString, CategoryPageUrl, CategoryId));

            if (HasParent)
            {
                html.AppendFormat(" in ");

                html.AppendFormat(HighlightedAnchor(ParentName, subString, CategoryPageUrl, ParentId));
            }

            return html.ToString();
        }

        /// <summary>
        /// Need Equals for unit testing
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as CategorySuggestion;

            if (other == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            return ((CategoryName == other.CategoryName) && (CategoryId == other.CategoryId) && 
                (ParentName == other.ParentName) && (ParentId == other.ParentId));
        }

        public override int GetHashCode()
        {
            return 23 * CategoryId.GetHashCode();
        }

        /// <summary>
        /// Serialise this object to a string efficiently
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CategoryName + "\n" + CategoryId.ToString(CultureInfo.InvariantCulture) + "\n" + 
                ParentName + "\n" + ParentId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserialse from a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CategorySuggestion Parse(string s)
        {
            string[] parts = s.Split('\n');

            return new CategorySuggestion(parts[0], int.Parse(parts[1]), parts[2], int.Parse(parts[3]));
        }
    }
}

