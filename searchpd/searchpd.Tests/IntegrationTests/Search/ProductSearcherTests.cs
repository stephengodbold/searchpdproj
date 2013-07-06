using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using searchpd.Models;
using searchpd.Repositories;
using searchpd.Search;
using NSubstitute;
using System.Linq;

namespace searchpd.Tests.IntegrationTests.Search
{
    [TestClass]
    public class ProductSearcherTests
    {
        private readonly string _lucenePath = "d:\\temp\\Lucene";

        private IProductSearcher _searcher;

        [TestInitialize]
        public void Initialize()
        {
            IEnumerable<ProductSearchResult> fakeDbProductResults = new List<ProductSearchResult>
                {
                    new ProductSearchResult("CABLE.7/0.20 X 3P OVERALL SCREENED","ELEEAS7203P",40354),
                    new ProductSearchResult("CABLE.7/0.20 X 3P OVERALL SCREENED","ELEEAS7203P",40355),
                    new ProductSearchResult("ELBOW.M16 90DEG NICKLE PLATED BRASS","FLEBM90-16M",40356),
                    new ProductSearchResult("ELBOW.M32 90DEG NICKLE PLATED BRASS 20'","FLEBM90-32M",40357),
                    new ProductSearchResult("CONDUIT.FSU(20MM) GALV-STL/PVC 10M BLACK","FLEFSU20B-10M",40358),
                    new ProductSearchResult("CONDUIT.FSU(25MM) GALV-STL/PVC 25M BLACK","FLEFSU25B-25M",40359)
                };

            var productRepository = Substitute.For<IProductRepository>();
            productRepository.GetAllProductSearchResults().ReturnsForAnyArgs(fakeDbProductResults);

            // ------------

            _searcher = new ProductSearcher(productRepository);

            _searcher.LoadProductStore(_lucenePath, 5, false);
        }

        [TestMethod]
        public void FindProductsBySearchTerm_InputIsNull_ReturnsEmptyCollection()
        {
            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm(null, 0, 10, out totalHits);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_InputIsEmpty_ReturnsEmptyCollection()
        {
            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("", 0, 10, out totalHits);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_NoMatches_ReturnsEmptyCollection()
        {
            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("xxxxyyyyy", 0, 10, out totalHits);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_MatchesOnlyProductDescriptions_ReturnsCorrectCollection()
        {
            //Arrange
            var expectedProductIDs = new[] { 40356, 40357 };

            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("PlaTed", 0, 10, out totalHits);

            // Assert
            Assert.IsTrue(ContainsProductIDs(results, expectedProductIDs));
        }

        [TestMethod]
        public void FindProductsBySearchTerm_MatchesBothProductCodesAndProductDescriptions_ReturnsCorrectCollection()
        {
            //Arrange
            var expectedProductIDs = new[] { 40354, 40355, 40357, 40358 };

            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("20", 0, 10, out totalHits);

            // Assert
            Assert.IsTrue(ContainsProductIDs(results, expectedProductIDs));
        }

        [TestMethod]
        public void FindProductsBySearchTerm_PagingEmptyPage_ReturnsEmptyCollection()
        {
            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("20", 0, 0, out totalHits).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_PagingFirstPage_ReturnsCorrectCollection()
        {
            //Arrange
            var expectedProductIDs = new[] { 40354, 40355 };

            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("20", 0, 2, out totalHits).ToList();

            // Assert
            Assert.IsTrue(ContainsProductIDs(results, expectedProductIDs));
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_PagingNextPage_ReturnsCorrectCollection()
        {
            //Arrange
            var expectedProductIDs = new[] { 40357, 40358 };

            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("20", 2, 2, out totalHits).ToList();

            // Assert
            Assert.IsTrue(ContainsProductIDs(results, expectedProductIDs));
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void FindProductsBySearchTerm_PagingNextPageTooManyAsked_ReturnsCorrectCollection()
        {
            //Arrange
            var expectedProductIDs = new[] { 40357, 40358 };

            // Act
            int totalHits;
            IEnumerable<ProductSearchResult> results = _searcher.FindProductsBySearchTerm("20", 2, 3, out totalHits).ToList();

            // Assert
            Assert.IsTrue(ContainsProductIDs(results, expectedProductIDs));
            Assert.AreEqual(2, results.Count());
        }

        /// <summary>
        /// Returns true if all given productIDs are represented in the results.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="productIDs"></param>
        /// <returns></returns>
        private bool ContainsProductIDs(IEnumerable<ProductSearchResult> results, int[] productIDs)
        {
            var joined = results.Join(productIDs, r => r.ProductId, p => p, (r, p) => p);
            return (joined.Count() != 0);
        }
    }
}
