using System.Configuration;
using System.IO;
using System.Web;

namespace main.Models
{
    public interface IConstants
    {
        string AutocompleteSearchUrl { get; }
        string AutocompleteRefreshUrl { get; }
        string LucenePath { get; }
        string AbsoluteLucenePath { get; }
        float ProductCodeBoost { get; }
        int NbrResultsPerPage { get; }
        string AutocompleteRefreshPassword { get; }
    }

    public class Constants : IConstants
    {
        public string AutocompleteSearchUrl
        {
            get { return ConfigurationManager.AppSettings["AutocompleteSearchUrl"]; }
        }

        public string AutocompleteRefreshUrl
        {
            get { return ConfigurationManager.AppSettings["AutocompleteRefreshUrl"]; }
        }

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

        // Boosting of product codes relative to product descriptions during product searches.
        // If this is 5, than matches on product codes are 5 times more important than matches in product descriptions.
        public float ProductCodeBoost
        {
            get { return float.Parse(ConfigurationManager.AppSettings["ProductCodeBoost"]); }
        }

        public int NbrResultsPerPage
        {
            get { return int.Parse(ConfigurationManager.AppSettings["NbrResultsPerPage"]); }
        }

        public string AutocompleteRefreshPassword
        {
            get { return ConfigurationManager.AppSettings["AutocompleteRefreshPassword"]; }
        }
    }
}
