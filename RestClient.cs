using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using TaxJar.Logging;
using TaxJar.Models;

namespace TaxJar
{
    public class RestClient
    {
        private static readonly ILog Log = LogProvider.For<RestClient>();
        private readonly string _authToken;
        protected readonly string TaxJarUrl = "https://api.taxjar.com/v2";

        public RestClient()
        {
        }

        public RestClient(string authToken)
        {
            _authToken = authToken;
        }

        protected async Task<T> GetAsync<T>(string resource, object queryParams = null) where T : class
        {
            Log.Trace("GET " + resource);
            try
            {
                var response = await new Url(TaxJarUrl)
                    .AppendPathSegment(resource)
                    .SetQueryParams(queryParams)
                    .WithDefaults()
                    .WithOAuthBearerToken(_authToken)
                    .GetAsync();
                Log.Trace(response.RequestMessage.ToString);
                var responseBody = await response.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        if (Debugger.IsAttached) Debugger.Break();
                    }
                };
                var responseDeserialized = JsonConvert.DeserializeObject<T>(responseBody, settings);
                return responseDeserialized;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TaxJarException("timeout", "Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                var response = await ex.Call.Response.Content.ReadAsStringAsync();
                var TaxJarEx = new TaxJarException("error", response)
                    {Method = "GET", Resource = resource, HttpStatus = ex.Call.HttpStatus};
                throw TaxJarEx;
            }
        }

        protected async Task<T> PostAsync<T>(string resource, object body)
        {
            Log.Trace("POST " + resource);
            try
            {
                var request = new Url(TaxJarUrl)
                    .AppendPathSegment(resource)
                    .WithDefaults()
                    .WithOAuthBearerToken(_authToken);
                return await request.PostUrlEncodedAsync(body)
                    .ReceiveJson<T>();
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TaxJarException("timeout", "Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                var response = await ex.Call.Response.Content.ReadAsStringAsync();
                var TaxJarEx = new TaxJarException("error", response)
                {
                    Method = "POST", Resource = resource, HttpStatus = ex.Call.HttpStatus, HttpMessage = ex.Message,
                    RequestBody = ex.Call.RequestBody
                };
                throw TaxJarEx;
            }
        }

        protected async Task<T> PutAsync<T>(string resource, object body = null)
        {
            Log.Trace("PUT " + resource);
            try
            {
                var request = new Url(TaxJarUrl)
                    .AppendPathSegment(resource)
                    .WithDefaults()
                    .WithOAuthBearerToken(_authToken);
                var response = await request.SendUrlEncodedAsync(HttpMethod.Put, body)
                    .ReceiveJson<T>();
                return response;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TaxJarException("timeout", "Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                var response = await ex.Call.Response.Content.ReadAsStringAsync();
                var TaxJarEx = new TaxJarException("error", response)
                {
                    Method = "PUT", Resource = resource, HttpStatus = ex.Call.HttpStatus, HttpMessage = ex.Message,
                    RequestBody = ex.Call.RequestBody
                };
                throw TaxJarEx;
            }
        }

        protected async Task DeleteAsync(string resource, object queryParams = null)
        {
            Log.Trace("DELETE " + resource);
            try
            {
                await new Url(TaxJarUrl)
                    .AppendPathSegment(resource)
                    .SetQueryParams(queryParams)
                    .WithDefaults()
                    .WithOAuthBearerToken(_authToken)
                    .DeleteAsync();
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TaxJarException("timeout", "Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                var response = await ex.Call.Response.Content.ReadAsStringAsync();
                var TaxJarEx = new TaxJarException("error", response)
                    {Method = "DELETE", Resource = resource, HttpStatus = ex.Call.HttpStatus, HttpMessage = ex.Message};
                throw TaxJarEx;
            }
        }

        protected async Task<T> DeleteAsync<T>(string resource, object queryParams = null)
        {
            Log.Trace("DELETE " + resource);
            try
            {
                var response = await new Url(TaxJarUrl)
                    .AppendPathSegment(resource)
                    .SetQueryParams(queryParams)
                    .WithDefaults()
                    .WithOAuthBearerToken(_authToken)
                    .DeleteAsync()
                    .ReceiveJson<T>();
                return response;
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TaxJarException("timeout", "Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                var response = await ex.Call.Response.Content.ReadAsStringAsync();
                var TaxJarEx = new TaxJarException("error", response)
                    {Method = "DELETE", Resource = resource, HttpStatus = ex.Call.HttpStatus, HttpMessage = ex.Message};
                throw TaxJarEx;
            }
        }
    }

    public static class UrlExtension
    {
        public static IFlurlRequest WithDefaults(this Url url)
        {
            return url
                .WithTimeout(10)
                .WithHeader("Accept", "application/json");
        }
    }
}