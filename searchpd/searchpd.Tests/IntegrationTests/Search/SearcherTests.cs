using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using searchpd.Models;
using searchpd.Repositories;
using searchpd.Search;
using NSubstitute;
using System.Linq;
using searchpd.Tests.ExtensionMethods;

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
            IEnumerable<CategoryHierarchy> fakeDbCategoryHierarchies = new List<CategoryHierarchy>
                {
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 200, Name = "Gamma-ray gun" },
                            Parent = new Category { CategoryID = 20, ParentID = 1, Name = "Space weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 101, Name = "X-ray gun 15'" },
                            Parent = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 100, Name = "X-ray gun 12'" },
                            Parent = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 20, ParentID = 1, Name = "Space weapons" },
                            Parent = null
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" },
                            Parent = new Category { CategoryID = 2, ParentID = 1, Name = "Earth weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 1, Name = "Earth weapons" },
                            Parent = null
                        }
                };

            var categoryRepository = Substitute.For<ICategoryRepository>();
            categoryRepository.GetAllHierarchies().ReturnsForAnyArgs(fakeDbCategoryHierarchies); 

            _searcher = new Searcher(categoryRepository);
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
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring(null);

            // Assert
            Assert.AreEqual(0, hierarchies.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_InputIsEmpty_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring("");

            // Assert
            Assert.AreEqual(0, hierarchies.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_NoMatches_ReturnsEmptyCollection()
        {
            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring("xxyyzz");

            // Assert
            Assert.AreEqual(0, hierarchies.Count());
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_MatchesTopLevelCategoryAtStart_ReturnsCollectionWithThatCategory()
        {
            // Arrange
            IEnumerable<CategoryHierarchy> expectedHierarchies = new List<CategoryHierarchy>
                {
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 1, Name = "Earth weapons" },
                            Parent = null
                        }
                };

            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring("Earth").ToList();

            // Assert
            Assert.IsTrue(expectedHierarchies.EqualTo(hierarchies));
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_MatchesTopLevelCategoryInMiddleDifferentCase_ReturnsCollectionWithThatCategory()
        {
            // Arrange
            IEnumerable<CategoryHierarchy> expectedHierarchies = new List<CategoryHierarchy>
                {
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 1, Name = "Earth weapons" },
                            Parent = null
                        }
                };
            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring("Arth").ToList();

            // Assert
            Assert.IsTrue(expectedHierarchies.EqualTo(hierarchies));
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_MatchesMultipleTopAndMidLevelCategoriesSubstringHasSpace_ReturnsCollectionWithThoseCategories()
        {
            // Arrange
            IEnumerable<CategoryHierarchy> expectedHierarchies = new List<CategoryHierarchy>
                {
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 1, Name = "Earth weapons" },
                            Parent = null
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 20, ParentID = 1, Name = "Space weapons" },
                            Parent = null
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" },
                            Parent = new Category { CategoryID = 2, ParentID = 1, Name = "Earth weapons" }
                        }
                };
            
            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring(" weapo").ToList();

            // Assert
            Assert.IsTrue(expectedHierarchies.EqualTo(hierarchies));
        }

        [TestMethod]
        public void FindCategoryHierarchiesBySubstring_MatchesMultipleNonTopLevelCategories_ReturnsCollectionWithThoseCategories()
        {
            // Arrange
            IEnumerable<CategoryHierarchy> expectedHierarchies = new List<CategoryHierarchy>
                {
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 100, Name = "X-ray gun 12'" },
                            Parent = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 101, Name = "X-ray gun 15'" },
                            Parent = new Category { CategoryID = 10, ParentID = 2, Name = "Laser weapons" }
                        },
                    new CategoryHierarchy
                        {
                            Category = new Category { CategoryID = 200, Name = "Gamma-ray gun" },
                            Parent = new Category { CategoryID = 20, ParentID = 1, Name = "Space weapons" }
                        }
                };

            // Act
            IEnumerable<CategoryHierarchy> hierarchies = _searcher.FindCategoryHierarchiesBySubstring("-ray").ToList();

            // Assert
            bool equal = expectedHierarchies.EqualTo(hierarchies);
            Assert.IsTrue(equal);
        }
    }
}
