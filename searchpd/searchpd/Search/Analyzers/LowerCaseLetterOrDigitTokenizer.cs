using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ext;
using Lucene.Net.Util;

namespace searchpd.Search.Lucene
{
    /// <summary>
    /// Same as the standard LetterOrDigitTokenizer (tokens consist of letters or digits), 
    /// except that it converts all letters to lowercase for case insensitive matching.
    /// </summary>
    public class LowerCaseLetterOrDigitTokenizer : LetterOrDigitTokenizer
    {
        public LowerCaseLetterOrDigitTokenizer(TextReader @in)
          : base(@in)
        {
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