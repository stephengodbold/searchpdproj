using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace searchpd.Search.Analyzers
{
    /// <summary>
    /// Same as the standard KeywordTokenizer, except that it converts all letters to uppercase for case insensitive matching.
    /// </summary>
    public class UpperCaseKeywordTokenizer : CharTokenizer
    {
        public UpperCaseKeywordTokenizer(TextReader @in)
          : base(@in)
        {
        }

        public UpperCaseKeywordTokenizer(AttributeSource source, TextReader @in)
          : base(source, @in)
        {
        }

        public UpperCaseKeywordTokenizer(AttributeFactory factory, TextReader @in)
          : base(factory, @in)
        {
        }

        /// <summary>
        /// Should return true if given char should go into a token.
        /// 
        /// Always return true, because we want everything in the one token.
        /// </summary>
        protected override bool IsTokenChar(char c)
        {
          return true;
        }

        /// <summary>
        /// This is called by CharTokenizer for each char.
        /// Convert it to upper case.
        /// </summary>
        protected override char Normalize(char c)
        {
            return char.ToUpper(c);
        }
    }
}