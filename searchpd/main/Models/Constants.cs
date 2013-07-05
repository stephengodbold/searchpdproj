using System.Configuration;

namespace main.Models
{
    public interface IConstants
    {
        string AutocompleteSearchApiUrl { get; }
        string AutocompleteRefreshApiUrl { get; }
        string LucenePath { get; }
        float ProductCodeBoost { get; }
    }

    public class Constants : IConstants
    {
        public string AutocompleteSearchApiUrl
        {
            get { return ConfigurationManager.AppSettings["AutocompleteSearchApiUrl"]; }
        }

        public string AutocompleteRefreshApiUrl
        {
            get { return ConfigurationManager.AppSettings["AutocompleteRefreshApiUrl"]; }
        }

        // Path relative to the root of the executing site where the Lucene files will be stored.
        public string LucenePath
        {
            get { return ConfigurationManager.AppSettings["LucenePath"]; }
        }

        // Boosting of product codes relative to product descriptions during product searches.
        // If this is 5, than matches on product codes are 5 times more important than matches in product descriptions.
        public float ProductCodeBoost
        {
            get { return float.Parse(ConfigurationManager.AppSettings["ProductCodeBoost"]); }
        }

    }
}
