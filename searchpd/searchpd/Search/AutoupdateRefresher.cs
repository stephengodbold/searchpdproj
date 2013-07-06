using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace searchpd.Search
{
    /// <summary>
    /// This component is used to tell the autoupdate site to refresh its suggestions from the database.
    /// You would run this from the main site, or some other site other than the autoupdate site.
    /// </summary>
    public interface IAutoupdateRefresher
    {
        string RefreshAutoupdate(string refreshUrl, string password);
    }

    public class AutoupdateRefresher : IAutoupdateRefresher
    {
        /// <summary>
        /// Sends refresh POST to autoupdate refresher endpoint
        /// </summary>
        /// <param name="refreshUrl">
        /// Url of the endpoint, to send the POST to.
        /// </param>
        /// <param name="password">
        /// Password to send in the password field.
        /// </param>
        /// <returns>
        /// Response from the refresher.
        /// </returns>
        public string RefreshAutoupdate(string refreshUrl, string password)
        {
            try
            {
                // Create request
                WebRequest request = WebRequest.Create(refreshUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                
                // Create POST data
                string postData = string.Format("password={0}", HttpUtility.UrlEncode(password));
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                // Send the request
                using(Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                using(WebResponse response = request.GetResponse())
                {
                    // Check that response is ok

                    var httpResponse = response as HttpWebResponse;

                    if (httpResponse == null)
                    {
                        throw new Exception("Response is not an http response");
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        return string.Format("Bad response from autocomplete server: {0}", httpResponse.StatusDescription);
                    }

                    // Read the response

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            throw new Exception("Cannot get response stream");
                        }

                        using (var reader = new StreamReader(responseStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            return responseFromServer;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
