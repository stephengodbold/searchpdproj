using System.Collections.Generic;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using searchpd.Models;
using searchpd.Repositories;
using searchpd.Search;
using NSubstitute;
using System.Linq;

namespace searchpd.Tests.IntegrationTests.Search
{
    [TestClass]
    public class SuggestionSearcherTests
    {
        private ISuggestionSearcher _searcher;

        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize]
        public void Initialize()
        {
            // Fake category data
            //
            // Gamma-ray gun | Space weapons
            // X-ray gun 12' | Laser weapons | Earth weapons
            // X-ray gun 15' | Laser weapons | Earth weapons
            IEnumerable<CategorySuggestion> fakeDbCategorySuggestions = new List<CategorySuggestion>
                {
                    new CategorySuggestion("Gamma-ray gun",200,"Space weapons",20),
                    new CategorySuggestion("X-ray gun 15'",101,"Laser weapons",10),
                    new CategorySuggestion("X-ray gun 12'",100,"Laser weapons",10),
                    new CategorySuggestion("Space weapons",20,null,0),
                    new CategorySuggestion("Laser weapons",10,"Earth weapons" ,2),
                    new CategorySuggestion("Earth weapons",10,null,0)
                };

            var categoryRepository = Substitute.For<ICategoryRepository>();
            categoryRepository.GetAllSuggestions().ReturnsForAnyArgs(fakeDbCategorySuggestions);

            // ------------

            IEnumerable<ProductSuggestion> fakeDbProductSuggestions = new List<ProductSuggestion>
                {
                    new ProductSuggestion("Xray8944",1),
                    new ProductSuggestion("Bray8946wk",2)
                };

            var productRepository = Substitute.For<IProductRepository>();
            productRepository.GetAllSuggestions().ReturnsForAnyArgs(fakeDbProductSuggestions);

            // ------------

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Cache.Returns(HttpRuntime.Cache);

            // ------------

            _searcher = new SuggestionSearcher(categoryRepository, productRepository);
        }

        /// <summary>
        ///Cleanup() is called once during test execution after
        ///test methods in this class have executed unless
        ///this test class' Initialize() method throws an exception.
        ///</summary>
        [TestCleanup]
        public void Cleanup()
        {
        }


        [TestMethod]
        public void FindSuggestionsBySubstring_InputIsNull_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring(null);

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_InputIsEmpty_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("");

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_NoMatches_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("xxyyzz");

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesTopLevelCategoryAtStart_ReturnsCollectionWithThatCategory()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<CategorySuggestion>
                {
                    new CategorySuggestion("Earth weapons",10,null,0)
                };

            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("Earth").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesTopLevelCategoryInMiddleDifferentCase_ReturnsCollectionWithThatCategory()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<CategorySuggestion>
                {
                    new CategorySuggestion("Earth weapons",10,null,0)
                };
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("Arth").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesMultipleTopAndMidLevelCategoriesSubstringHasSpace_ReturnsCollectionWithThoseCategories()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<CategorySuggestion>
                {
                    new CategorySuggestion("Earth weapons",10,null,0),
                    new CategorySuggestion("Space weapons",20,null,0),
                    new CategorySuggestion("Laser weapons",10,"Earth weapons" ,2)
                };
            
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring(" weapo").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesMultipleNonTopLevelCategories_ReturnsCollectionWithThoseCategories()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<CategorySuggestion>
                {
                    new CategorySuggestion("X-ray gun 12'",100,"Laser weapons",10),
                    new CategorySuggestion("X-ray gun 15'",101,"Laser weapons",10),
                    new CategorySuggestion("Gamma-ray gun",200,"Space weapons",20)
                };

            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("-ray").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesOnlyProductCodes_ReturnsCollectionWithThoseSuggestions()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<ISuggestion>
                {
                    new ProductSuggestion("Bray8946wk",2),
                    new ProductSuggestion("Xray8944",1)
                };

            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("y894").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }

        [TestMethod]
        public void FindSuggestionsBySubstring_MatchesBothCategorySuggestionsAndProductCodes_ReturnsCollectionWithThoseSuggestions()
        {
            // Arrange
            IEnumerable<ISuggestion> expectedSuggestions = new List<ISuggestion>
                {
                    new CategorySuggestion("X-ray gun 12'",100,"Laser weapons",10),
                    new CategorySuggestion("X-ray gun 15'",101,"Laser weapons",10),
                    new CategorySuggestion("Gamma-ray gun",200,"Space weapons",20),
                    new ProductSuggestion("Bray8946wk",2),
                    new ProductSuggestion("Xray8944",1)
                };

            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("ray").ToList();

            // Assert
            Assert.IsTrue(expectedSuggestions.SequenceEqual(suggestions));
        }
    }
}
