using System.IO;
using Lucene.Net.Analysis.Ext;

namespace searchpd.Search.Analyzers
{
    /// <summary>
    /// Same as the standard LetterOrDigitTokenizer (tokens consist of letters or digits), 
    /// except that it converts all letters to upper case for case insensitive matching.
    /// </summary>
    public class UpperCaseLetterOrDigitTokenizer : LetterOrDigitTokenizer
    {
        public UpperCaseLetterOrDigitTokenizer(TextReader @in)
          : base(@in)
        {
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