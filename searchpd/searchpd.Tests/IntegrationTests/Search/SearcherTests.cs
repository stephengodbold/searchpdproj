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
    public class SearcherTests
    {
        private ISearcher _searcher;

        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize()]
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

            _searcher = new Searcher(categoryRepository, null,null);//##################
        }

        /// <summary>
        ///Cleanup() is called once during test execution after
        ///test methods in this class have executed unless
        ///this test class' Initialize() method throws an exception.
        ///</summary>
        [TestCleanup()]
        public void Cleanup()
        {
        }


        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_InputIsNull_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring(null);

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_InputIsEmpty_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("");

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_NoMatches_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<ISuggestion> suggestions = _searcher.FindSuggestionsBySubstring("xxyyzz");

            // Assert
            Assert.AreEqual(0, suggestions.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_MatchesTopLevelCategoryAtStart_ReturnsCollectionWithThatCategory()
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
        public void FindCategoryHierarchiesBySubstring_MatchesTopLevelCategoryInMiddleDifferentCase_ReturnsCollectionWithThatCategory()
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
        public void FindCategoryHierarchiesBySubstring_MatchesMultipleTopAndMidLevelCategoriesSubstringHasSpace_ReturnsCollectionWithThoseCategories()
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
        public void FindCategoryHierarchiesBySubstring_MatchesMultipleNonTopLevelCategories_ReturnsCollectionWithThoseCategories()
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
    }
}
