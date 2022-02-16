using BackendProductConfigurator.App_Code;
using Model.Wrapper;
using System.Net.Http.Json;
using System.Text;

namespace BackendProductConfigurator.Controllers
{
    public static class ADBAccess<T> where T : class
    {
        public static async Task<List<T>> GetValues(string language, string address, string api)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
        }
        public static async Task<HttpResponseMessage> PutValue(string language, string address, string api, T value)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.PutAsJsonAsync($"{address}{api}", value);
        }
        public static async Task<HttpResponseMessage> PostValue(string language, string address, string api, T value)
        {
            HttpClient Http = GenerateHttpClient(language);

            return await Http.PostAsJsonAsync($"{address}{api}", value);
        }
        public static async Task<HttpResponseMessage> DeleteValue(string language, string address, string api, T identifier)
        {
            HttpClient Http = GenerateHttpClient(language);

            StringBuilder sb = new StringBuilder(address).Append(api).Append('/');
            if(typeof(T) == typeof(SavedConfigWrapper))
            {
                sb.Append((identifier as SavedConfigWrapper).ConfigId).Append('/').Append((identifier as SavedConfigWrapper).SavedName);
            }
            else if(typeof(T) == typeof(ConfigurationDeleteWrapper))
            {
                sb.Append((identifier as ConfigurationDeleteWrapper).ConfigId);
            }

            return await Http.DeleteAsync(sb.ToString());
        }
        private static HttpClient GenerateHttpClient(string language)
        {
            HttpClientHandler handler = new HttpClientHandler();

            if(!GlobalValues.Secure)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
            }

            HttpClient Http = new HttpClient(handler);

            Http.DefaultRequestHeaders.Add("Accept-Language", language);

            return Http;
        }
    }
}
