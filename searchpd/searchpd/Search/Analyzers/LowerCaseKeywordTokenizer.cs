using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace searchpd.Search.Lucene
{
    /// <summary>
    /// Same as the standard KeywordTokenizer, except that it converts all letters to lowercase for case insensitive matching.
    /// </summary>
    public class LowerCaseKeywordTokenizer : CharTokenizer
    {
        /// <summary>
            /// Construct a new LowerCaseKeywordTokenizer.
        /// </summary>
        public LowerCaseKeywordTokenizer(TextReader @in)
          : base(@in)
        {
        }

        /// <summary>
        /// Construct a new LowerCaseKeywordTokenizer using a given <see cref="T:Lucene.Net.Util.AttributeSource"/>.
        /// </summary>
        public LowerCaseKeywordTokenizer(AttributeSource source, TextReader @in)
          : base(source, @in)
        {
        }

        /// <summary>
        /// Construct a new LowerCaseKeywordTokenizer using a given <see cref="T:Lucene.Net.Util.AttributeSource.AttributeFactory"/>.
        /// </summary>
        public LowerCaseKeywordTokenizer(AttributeFactory factory, TextReader @in)
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
        /// Convert it to lower case.
        /// </summary>
        protected override char Normalize(char c)
        {
            return char.ToLower(c);
        }
    }
}