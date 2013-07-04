using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace main.Models
{
    public interface IConstants
    {
        string AutocompleteSearchApiUrl { get; }
    }

    public class Constants : IConstants
    {
        public string AutocompleteSearchApiUrl
        {
            get { return ConfigurationManager.AppSettings["AutocompleteSearchApiUrl"]; }
        }
    }
}
