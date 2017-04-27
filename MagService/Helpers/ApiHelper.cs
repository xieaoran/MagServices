#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MagService.Exceptions;
using Newtonsoft.Json;

#endregion

namespace MagService.Helpers
{
    public static class ApiHelper
    {
        public static T Get<T>(string apiUrl, IDictionary<string, object> getContent)
        {
            var contentStrings = getContent?.Select(content => $"{content.Key}={content.Value}") ?? new string[0];
            var completeUrl = apiUrl + "?" + string.Join("&", contentStrings);
            var webRequest = (HttpWebRequest) WebRequest.Create(completeUrl);
            webRequest.Timeout = AppConfig.Timeout;
            webRequest.Method = "GET";
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            var webResponseStream = webResponse.GetResponseStream();
            if (webResponseStream == null) throw Network.LinkFailed;
            var webResponseReader = new StreamReader(webResponseStream);
            var responseJson = webResponseReader.ReadToEnd();
            webRequest.Abort();
            webResponse.Close();
            webResponseReader.Close();
            return JsonConvert.DeserializeObject<T>(responseJson, AppConfig.JsonSerializeSettings);
        }
    }
}