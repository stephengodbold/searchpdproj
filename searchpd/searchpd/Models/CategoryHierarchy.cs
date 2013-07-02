using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace searchpd.Models
{
    /// <summary>
    /// Represents a category and its parent.
    /// Only the Name and CategoryID properties are used. Do not rely on any other properties being used.
    /// 
    /// If the parent of a category is the top most category WEBONLINE with ParentID equals 0, the Parent property will
    /// be null.
    /// </summary>
    public class CategoryHierarchy
    {
        public Category Category { get; set; }
        public Category Parent { get; set; }
    }
}

