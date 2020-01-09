using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace FlightApi.Util
{
    public static class HttpService
    {
        public static async Task<string> HandleHttpGetRequest(string apiEndpoint)
        {
            HttpClient httpClient = new HttpClient();

            return await httpClient.GetStringAsync(apiEndpoint);
        }

        public static async Task<string> HandleHttpGetRequest(string apiEndpoint, Dictionary<string, string> requestHeadersValue)
        {
            HttpClient httpClient = new HttpClient();

            foreach (KeyValuePair<string, string> requestHeaderValue in requestHeadersValue)
            {
                var headerKey = requestHeaderValue.Key;
                var headerValue = requestHeaderValue.Value;

                httpClient.DefaultRequestHeaders.Add(headerKey, headerValue);
            }

            return await httpClient.GetStringAsync(apiEndpoint);
        }
    }
}
