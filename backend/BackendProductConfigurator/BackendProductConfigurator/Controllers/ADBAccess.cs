using System.Net.Http.Json;

namespace BackendProductConfigurator.Controllers
{
    public static class ADBAccess<T> where T : class
    {
        private static HttpClientHandler handler;

        private static HttpClient Http;
        public static async Task<List<T>> GetValues(string address, string api)
        {
            handler = new HttpClientHandler();

            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }; 

            Http = new HttpClient(handler);

            return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
        }
        public static async Task<HttpResponseMessage> PostValue(string address, string api, T value)
        {
            return await Http.PostAsJsonAsync($"{address}{api}", value);
        }
    }
}
