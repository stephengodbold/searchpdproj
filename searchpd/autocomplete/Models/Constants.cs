using System.Configuration;
using System.IO;
using System.Web;

namespace autocomplete.Models
{
    public interface IConstants
    {
        string LucenePath { get; }
        string AbsoluteLucenePath { get; }
        string AutocompleteRefreshPassword { get; }
    }

    public class Constants : IConstants
    {
        // Path relative to the root of the executing site where the Lucene files will be stored.
        public string LucenePath
        {
            get { return ConfigurationManager.AppSettings["LucenePath"]; }
        }

        // Absolute path where the Lucene files will be stored.
        public string AbsoluteLucenePath
        {
            get
            {
                string rootPath = HttpRuntime.AppDomainAppPath;
                string absoluteLucenePath = Path.Combine(rootPath, LucenePath);
                return absoluteLucenePath;
            }
        }

        public string AutocompleteRefreshPassword
        {
            get { return ConfigurationManager.AppSettings["AutocompleteRefreshPassword"]; }
        }
    }
}
