using System.Net.Http.Json;

namespace BackendProductConfigurator.Controllers
{
    public static class ADBAccess<T> where T : class
    {
        public static async Task<List<T>> GetValues(string language, string address, string api)
        {
            HttpClientHandler handler;

            HttpClient Http;
            handler = new HttpClientHandler();

            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }; 

            Http = new HttpClient(handler);

            Http.DefaultRequestHeaders.Add("Accept-language", language);

            return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
        }
        public static async Task<HttpResponseMessage> PostValue(string language, string address, string api, T value)
        {
            HttpClientHandler handler;

            HttpClient Http;
            handler = new HttpClientHandler();

            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            Http = new HttpClient(handler);

            Http.DefaultRequestHeaders.Add("Accept-language", language);

            return await Http.PostAsJsonAsync($"{address}{api}", value);
        }
    }
}
