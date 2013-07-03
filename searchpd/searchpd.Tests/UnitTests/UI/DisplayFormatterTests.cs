using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using searchpd.UI;

namespace searchpd.Tests.UnitTests.UI
{
    [TestClass]
    public class DisplayFormatterTests
    {
        private IDisplayFormatter _displayFormatter;

        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize()]
        public void Initialize()
        {
            _displayFormatter = new DisplayFormatter();
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
        public void SubstringHighlighted_InputIsNull_ReturnsNull()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted(null, "xyz");

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void SubstringHighlighted_InputIsEmpty_ReturnsEmpty()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("", "xyz");

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void SubstringHighlighted_NoMatchNoHtml_ReturnsInput()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcdefg", "xyz");

            // Assert
            Assert.AreEqual("abcdefg", result);
        }

        [TestMethod]
        public void SubstringHighlighted_NoMatchHasHtml_ReturnsEscapedInput()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("a<b>c&defg", "xyz");

            // Assert
            Assert.AreEqual("a&lt;b&gt;c&amp;defg", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchAtStartNoHtml_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcdefg", "abc");

            // Assert
            Assert.AreEqual("<b>abc</b>defg", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchAtEndNoHtml_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcdefg", "efg");

            // Assert
            Assert.AreEqual("abcd<b>efg</b>", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchInMiddleNoHtml_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcdefg", "def");

            // Assert
            Assert.AreEqual("abc<b>def</b>g", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchHasHtmlOutsideMatch_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("a<b>cdefg", "def");

            // Assert
            Assert.AreEqual("a&lt;b&gt;c<b>def</b>g", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchHasHtmlInsideMatch_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcd&efg", "d&ef");

            // Assert
            Assert.AreEqual("abc<b>d&amp;ef</b>g", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchHasHtmlInsideMatchAtStart_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcd&efg", "&ef");

            // Assert
            Assert.AreEqual("abcd<b>&amp;ef</b>g", result);
        }

        [TestMethod]
        public void SubstringHighlighted_HasMatchHasHtmlInsideMatchAtEnd_ReturnsInputWithHighlightedSubstring()
        {
            // Act
            string result = _displayFormatter.SubstringHighlighted("abcdef&g", "ef&");

            // Assert
            Assert.AreEqual("abcd<b>ef&amp;</b>g", result);
        }
    }
}


