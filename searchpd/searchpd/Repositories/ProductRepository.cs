using System.Collections.Generic;
using System.Linq;
using searchpd.Models;

namespace searchpd.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<ProductSuggestion> GetAllSuggestions();
        Product GetProductById(int productId);
    }

    public class ProductRepository : IProductRepository
    {
        /// <summary>
        /// Gets all category hierarchies from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductSuggestion> GetAllSuggestions()
        {
            using (var context = new searchpdEntities())
            {
                IEnumerable<ProductSuggestion> suggestions =
                    context.Products.Select(p => new ProductSuggestion {
                        ProductCode = p.Code, 
                        ProductId = p.ProductID
                    }).ToList();

                return suggestions;
            }
        }

        /// <summary>
        /// Returns a product given its id, or null if the id does not match a product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product GetProductById(int productId)
        {
            using (var context = new searchpdEntities())
            {
                Product product = context.Products.Find(productId);

                return product;
            }
        }
    }
}

