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
    public class CategorySuggestionTests
    {
        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize]
        public void Initialize()
        {
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
        public void Equals_OtherIsNull_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
                {
                    CategoryName = "Torches & Accs",
                    CategoryId = 36864,
                    ParentName = "Batteries Torches",
                    ParentId = 36856
                };

            // Act
            bool equal = categorySuggestion.Equals(null);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherNameIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Headlights",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherIdIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36865,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherParentNameIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches and Lights",
                ParentId = 36856
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherParentIdIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36857
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherHasNoParent_ReturnsFalse()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "",
                ParentId = 0
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherIsEqual_ReturnsTrue()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void Equals_OtherIsEqualNoParent_ReturnsTrue()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "",
                ParentId = 0
            };

            var otherCategorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "",
                ParentId = 0
            };

            // Act
            bool equal = categorySuggestion.Equals(otherCategorySuggestion);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ParseToString_ToStringThenParse_ReturnsSame()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            // Act
            string serialised = categorySuggestion.ToString();
            var deserialised = CategorySuggestion.Parse(serialised);

            // Assert
            Assert.IsTrue(categorySuggestion.Equals(deserialised));
            Assert.AreEqual(categorySuggestion.HasParent, deserialised.HasParent);
        }

        [TestMethod]
        public void ParseToString_ToStringThenParseNoParent_ReturnsSame()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "",
                ParentId = 0
            };

            // Act
            string serialised = categorySuggestion.ToString();
            var deserialised = CategorySuggestion.Parse(serialised);

            // Assert
            Assert.IsTrue(categorySuggestion.Equals(deserialised));
            Assert.AreEqual(categorySuggestion.HasParent, deserialised.HasParent);
        }

        [TestMethod]
        public void HasParent_HasParent_ReturnsTrue()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "Batteries Torches",
                ParentId = 36856
            };

            // Act

            // Assert
            Assert.IsTrue(categorySuggestion.HasParent);
        }

        [TestMethod]
        public void HasParent_HasNoParent_ReturnsTrue()
        {
            // Arrange
            var categorySuggestion = new CategorySuggestion
            {
                CategoryName = "Torches & Accs",
                CategoryId = 36864,
                ParentName = "",
                ParentId = 0
            };

            // Act

            // Assert
            Assert.IsFalse(categorySuggestion.HasParent);
        }
    }
}
