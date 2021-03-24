using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApiParser.Data
{
    public class HttpDataGetter : IDataGetter
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public HttpDataGetter(IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            _httpClient = httpClientFactory.CreateClient(nameof(App));

            if (!string.IsNullOrEmpty(_appSettings.AuthenticationScheme) &&
                !string.IsNullOrEmpty(_appSettings.AuthenticationValue))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(_appSettings.AuthenticationScheme, _appSettings.AuthenticationValue);
            }
        }

        public async Task<string> GetData(string urlData)
        {
            string url = string.Format(_appSettings.UrlFormat, urlData);
            return await _httpClient.GetStringAsync(url).ConfigureAwait(false);
        }
    }
}