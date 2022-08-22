using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Application.Helpers.External
{
    public static class HttpClientExtensions
    {
        public static async Task<T?> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(dataAsString);
        }

        public static Task<HttpResponseMessage> GetAsJson(this HttpClient httpClient, string url, IConfiguration config, ResourceParameter? parameters = null)
        {
            url = UrlBuilder(url, parameters);

            httpClient.DefaultRequestHeaders.Add("api-key", config["api-key"]);

            return httpClient.GetAsync(url);
        }

        private static string UrlBuilder(string url, ResourceParameter? parameters)
        {
            if (parameters is not null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.Search))
                {
                    url = $"{url}?search={parameters.Search}";
                }

                if (parameters.PageSize != default && parameters.PageSize != null)
                {
                    if (url.Contains('?'))
                    {
                        url = $"{url}&page_size={parameters.PageSize}";
                    }
                    else
                    {
                        url = $"{url}?page_size={parameters.PageSize}";
                    }

                }
            }

            return url;
        }
    }
}
