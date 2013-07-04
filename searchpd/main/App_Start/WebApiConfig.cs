using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace main
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Make subString into a catch all, because the user may enter a /
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{*escapedSubString}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Get Web API to return string by default, rather than XML.
            // Client can still explicitly request XML via request header.
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
