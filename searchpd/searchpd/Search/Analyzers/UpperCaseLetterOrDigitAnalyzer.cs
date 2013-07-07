using System.IO;
using Lucene.Net.Analysis;

namespace searchpd.Search.Analyzers
{
    /// <summary>
    /// Same as KeywordAnalyzer, except that all letters are converted to uppercase.
    /// </summary>
    public class UpperCaseLetterOrDigitAnalyzer : Analyzer
    {
        public UpperCaseLetterOrDigitAnalyzer()
        {
          SetOverridesTokenStreamMethod<KeywordAnalyzer>();
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new UpperCaseLetterOrDigitTokenizer(reader);
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
          if (overridesTokenStreamMethod)
            return TokenStream(fieldName, reader);
          var tokenizer = (Tokenizer) PreviousTokenStream;
          if (tokenizer == null)
          {
              tokenizer = new UpperCaseLetterOrDigitTokenizer(reader);
            PreviousTokenStream = tokenizer;
          }
          else
            tokenizer.Reset(reader);
          return tokenizer;
        }
    }
}