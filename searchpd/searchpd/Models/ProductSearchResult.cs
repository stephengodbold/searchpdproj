namespace searchpd.Models
{
    /// <summary>
    /// Represents a single result of a product search.
    /// </summary>
    public class ProductSearchResult : DisplayObject
    {
        private const string ProductPageUrl = "/Home/Products/{0}";

        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        public int ProductId { get; set; } // Be sure never to update this after object creation, it is used as the hash code.

        public ProductSearchResult()
        {
        }

        public ProductSearchResult(string productDescription, string productCode, int productId)
        {
            ProductDescription = productDescription;
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
            string html = "<p>" + HighlightedAnchor(ProductCode, subString, ProductPageUrl, ProductId) + 
                "<br />" + 
                SubstringHighlighted(ProductDescription, subString) + "</p>";
            return html;
        }

        /// <summary>
        /// Need Equals for unit testing
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as ProductSearchResult;

            if (other == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            return ((ProductDescription == other.ProductDescription) && 
                    (ProductCode == other.ProductCode) && 
                    (ProductId == other.ProductId));
        }

        public override int GetHashCode()
        {
            return 23 * ProductId.GetHashCode();
        }
    }
}

