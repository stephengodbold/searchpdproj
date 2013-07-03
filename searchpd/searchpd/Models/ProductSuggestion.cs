using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace searchpd.Models
{
    /// <summary>
    /// Represents a product suggestion.
    /// 
    /// There are over 40,000 of these. Try to keep its memory footprint low.
    /// </summary>
    public class ProductSuggestion : DisplayObject, ISuggestion
    {
        private const string ProductPageUrl = "/Home/Products/{0}";

        public string ProductCode { get; set; }
        public int ProductId { get; set; } // Be sure never to update this after object creation, it is used as the hash code.

        public ProductSuggestion()
        {
        }

        public ProductSuggestion(string productCode, int productId)
        {
            ProductCode = productCode;
            ProductId = productId;
        }

        /// <summary>
        /// Returns the html representation of a product suggestion
        /// </summary>
        /// <param name="subString">
        /// Sub string to highlight in the html. Pass null to not hightlight anything.
        /// </param>
        /// <returns></returns>
        public string ToHtml(string subString)
        {
            string html = HighlightedAnchor(ProductCode, subString, ProductPageUrl, ProductId);
            return html;
        }

        /// <summary>
        /// Need Equals for unit testing
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as ProductSuggestion;

            if (other == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            return ((ProductCode == other.ProductCode) && (ProductId == other.ProductId));
        }

        public override int GetHashCode()
        {
            return 23 * ProductId.GetHashCode();
        }
    }
}

