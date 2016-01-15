using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace kcUpdater.Classes
{
    /// <summary>
    /// handle simple web requests
    /// </summary>
    internal class WebRequest
    {
        /// <summary>
        /// default timeout for web requests
        /// </summary>
        public static int TimeOut = 1000;

        /// <summary>
        /// default useragend for web requests
        /// </summary>
        public static string UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0;.NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; InfoPath.2;.NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";

        private static WebResponse _lastResponse;

        private static HttpWebRequest GetRequest(string uri, string userAgent, int timeOut)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            httpWebRequest.Timeout = timeOut;
            httpWebRequest.UserAgent = UserAgent;

            return httpWebRequest;
        }

        /// <summary>
        /// Do a simple web request
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="timeOut">timeout to overwrite the default one</param>
        public static void Request(string uri, int timeOut)
        {
            HttpWebRequest httpWebRequest = GetRequest(uri, UserAgent, timeOut);

            try
            {
                _lastResponse = httpWebRequest.GetResponse();
            } catch (WebException ex)
            {
                _lastResponse = null;
                throw new Exceptions.WebRequestFailedException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Do a simple web request
        /// </summary>
        /// <param name="uri">target domain</param>
        public static void Request(string uri)
        {
            Request(uri, TimeOut);
        }

        /// <summary>
        /// Do a simple web request
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="timeOut">timeout to overwrite the default one</param>
        public static void Request(Uri uri, int timeOut)
        {
            Request(uri.AbsoluteUri, timeOut);
        }

        /// <summary>
        /// Do a simple web request
        /// </summary>
        /// <param name="uri">target domain</param>
        public static void Request(Uri uri)
        {
            Request(uri.AbsoluteUri, TimeOut);
        }

        /// <summary>
        /// Indicates if the last web request was successfull
        /// </summary>
        /// <returns></returns>
        public static bool LastRequestSucceed()
        {
            if (_lastResponse == null) return false;
            HttpWebResponse httpWebResponse = (HttpWebResponse)_lastResponse;
            return httpWebResponse.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Gets the last received web request response string
        /// </summary>
        /// <returns></returns>
        public static string LastContent()
        {
            if (_lastResponse == null) return string.Empty;
            Stream responseStream = _lastResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Sends a POST web request in json format
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="timeOut">timeout to overwrite the default one</param>
        /// <param name="obj">object to send as json</param>
        public static void Post(string uri, int timeOut, object obj)
        {
            HttpWebRequest httpWebRequest = GetRequest(uri, UserAgent, timeOut);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JsonObject.Instance.WriteToStream(streamWriter, obj);
                }
                _lastResponse = httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                throw new Exceptions.WebRequestFailedException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Sends a POST web request in json format
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="obj">object to send as json</param>
        public static void Post(string uri, object obj)
        {
            Post(uri, TimeOut, obj);
        }

        /// <summary>
        /// Sends a POST web request in json format
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="timeOut">timeout to overwrite the default one</param>
        /// <param name="obj">object to send as json</param>
        public static void Post(Uri uri, int timeOut, object obj)
        {
            Post(uri.AbsoluteUri, timeOut, obj);
        }

        /// <summary>
        /// Sends a POST web request in json format
        /// </summary>
        /// <param name="uri">target domain</param>
        /// <param name="obj">object to send as json</param>
        public static void Post(Uri uri, object obj)
        {
            Post(uri.AbsoluteUri, TimeOut, obj);
        }
    }
}
