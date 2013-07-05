using System.Globalization;

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

        public string SortedName { get { return ProductCode; }  }

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

        /// <summary>
        /// Serialise this object to a string efficiently
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ProductCode + "\n" + ProductId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Deserialse from a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ProductSuggestion Parse(string s)
        {
            string[] parts = s.Split('\n');

            return new ProductSuggestion(parts[0], int.Parse(parts[1]));
        }
    }
}

