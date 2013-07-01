using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using searchpd;
using searchpd.Controllers;

namespace searchpd.Tests.Controllers
{
    [TestClass]
    public class SearchControllerTest
    {
        [TestMethod]
        public void GetById()
        {
            // Arrange
            var controller = new SearchController();

            // Act
            string result = controller.GetByName("");

            // Assert
            Assert.AreEqual("value", result);
        }

    }
}
