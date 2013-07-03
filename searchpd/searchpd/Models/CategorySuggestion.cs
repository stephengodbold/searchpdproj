﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public string CategoryName { get; private set; }
        public int CategoryId { get; private set; }
        
        // "Parent" refers to the parent category of this category.
        // If there is no parent, ParentName will be null
        public string ParentName { get; private set; }
        public int ParentId { get; private set; }

        public bool HasParent { get { return (ParentName != null); } }

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
        /// Factory to create a new CategorySuggestion.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="categoryId"></param>
        /// <param name="parentName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static CategorySuggestion Create(string categoryName, int categoryId, string parentName, int parentId)
        {
            return new CategorySuggestion
                {
                    CategoryName = categoryName,
                    CategoryId = categoryId,
                    ParentId = parentId,
                    ParentName = parentName
                };
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

            return ((CategoryName == other.CategoryName) && (CategoryId == other.CategoryId) && (ParentName == other.ParentName) && (ParentId == other.ParentId));
        }

        public override int GetHashCode()
        {
            return CategoryId;
        }
    }
}
