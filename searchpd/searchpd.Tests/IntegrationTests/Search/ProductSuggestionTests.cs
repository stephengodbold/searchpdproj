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
    public class ProductSuggestionTests
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
            var productSuggestion = new ProductSuggestion
                {
                    ProductCode = "FLEBM90-32M",
                    ProductId = 36864
                };

            // Act
            bool equal = productSuggestion.Equals(null);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherNameIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var productSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36864
            };

            var otherProductSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-344M",
                ProductId = 36864
            };

            // Act
            bool equal = productSuggestion.Equals(otherProductSuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherIdIsNotEqual_ReturnsFalse()
        {
            // Arrange
            var productSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36864
            };

            var otherProductSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36865
            };

            // Act
            bool equal = productSuggestion.Equals(otherProductSuggestion);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void Equals_OtherIsEqual_ReturnsTrue()
        {
            // Arrange
            var productSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36864
            };

            var otherProductSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36864
            };

            // Act
            bool equal = productSuggestion.Equals(otherProductSuggestion);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ParseToString_ToStringThenParse_ReturnsSame()
        {
            // Arrange
            var productSuggestion = new ProductSuggestion
            {
                ProductCode = "FLEBM90-32M",
                ProductId = 36864
            };

            // Act
            string serialised = productSuggestion.ToString();
            var deserialised = ProductSuggestion.Parse(serialised);

            // Assert
            Assert.IsTrue(productSuggestion.Equals(deserialised));
        }
    }
}
