using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace searchpd.UI
{
    public interface IDisplayFormatter
    {
        string SubstringHighlighted(string input, string subString);
        string HighlightedAnchor(string linkName, string subString, string urlFormat, params Object[] urlParams);
    }

    public class DisplayFormatter : IDisplayFormatter
    {
        /// <summary>
        /// 1) escapes all html in the given input string
        /// 2) if subString appears in input, it is highlighted via bolding tags.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="subString"></param>
        /// <returns>
        /// Escaped and bolded result.
        /// </returns>
        public string SubstringHighlighted(string input, string subString)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(subString))
            {
                return input;
            }

            // To highlight the sub string, we'll insert <b> tags. But we don't want to escape those tags.
            // Yet we need to find the sub string before doing any escaping.
            // So, first mark the insertion points for the <b> and </b> tags, then escape, then insert the tags.

            int subStringIdx = input.IndexOf(subString, StringComparison.OrdinalIgnoreCase);
            if (subStringIdx < 0)
            {
                return HttpUtility.HtmlEncode(input);
            }

            const string highlightStartMarker = "\r";
            const string highlightEndMarker = "\n";

            string markedString = input.Insert(subStringIdx + subString.Length, highlightEndMarker);
            markedString = markedString.Insert(subStringIdx, highlightStartMarker);

            string escapedString = HttpUtility.HtmlEncode(markedString);

            string highlightedString = escapedString.Replace(highlightStartMarker, "<b>");
            highlightedString = highlightedString.Replace(highlightEndMarker, "</b>");

            return highlightedString;
        }

        /// <summary>
        /// Creates an html anchor element.
        /// 
        /// urlFormat and urlParams are combined to form the url.
        /// </summary>
        /// <param name="linkName"></param>
        /// <param name="subString">
        /// If this appears in linkName, it will be highlighted via bolding tags.
        /// </param>
        /// <param name="urlFormat"></param>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public string HighlightedAnchor(string linkName, string subString, string urlFormat, params Object[] urlParams)
        {
            string highlightedLinkName = SubstringHighlighted(linkName, subString);
            string url = string.Format(urlFormat, urlParams);
            string anchorHtml = string.Format(@"<a href=""{0}"">{1}</a>", url, highlightedLinkName);

            return anchorHtml;
        }
    }
}

