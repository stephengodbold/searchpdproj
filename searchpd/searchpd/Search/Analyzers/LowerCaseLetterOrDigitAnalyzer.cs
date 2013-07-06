using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;

namespace searchpd.Search.Lucene
{
    /// <summary>
    /// Same as KeywordAnalyzer, except that all letters are converted to lowercase.
    /// </summary>
    public class LowerCaseLetterOrDigitAnalyzer : Analyzer
    {
        public LowerCaseLetterOrDigitAnalyzer()
        {
          SetOverridesTokenStreamMethod<KeywordAnalyzer>();
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseLetterOrDigitTokenizer(reader);
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
          if (this.overridesTokenStreamMethod)
            return this.TokenStream(fieldName, reader);
          var tokenizer = (Tokenizer) this.PreviousTokenStream;
          if (tokenizer == null)
          {
              tokenizer = (Tokenizer)new LowerCaseLetterOrDigitTokenizer(reader);
            this.PreviousTokenStream = (object) tokenizer;
          }
          else
            tokenizer.Reset(reader);
          return (TokenStream) tokenizer;
        }
    }
}