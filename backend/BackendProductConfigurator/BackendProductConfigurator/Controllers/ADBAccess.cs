using System.Net.Http.Json;

namespace BackendProductConfigurator.Controllers
{
    public static class ADBAccess<T> where T : class
    {
        public static async Task<List<T>> GetValues(string language, string address, string api)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
        }
        public static async Task<HttpResponseMessage> PostValue(string language, string address, string api, T value)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.PostAsJsonAsync($"{address}{api}", value);
        }
        public static async Task<HttpResponseMessage> DeleteValue(string language, string address, string api, string id)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.DeleteAsync($"{address}{api}/{id}");
        }
        private static HttpClient GenerateHttpClient(string language)
        {
            HttpClientHandler handler = new HttpClientHandler();

            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            HttpClient Http = new HttpClient(handler);

            Http.DefaultRequestHeaders.Add("Accept-Language", language);

            return Http;
        }
    }
}
