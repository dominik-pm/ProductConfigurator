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
            try
            {
                HttpClient Http = GenerateHttpClient(language);

                return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task PutValue(string language, string address, string api, T value)
        {
            try
            {
                HttpClient Http = GenerateHttpClient(language);

                if ((int)Http.PutAsJsonAsync($"{address}{api}", value).Result.StatusCode != 200)
                {
                    throw new Exception("Put failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async void PostValue(string language, string address, string api, T value)
        {
            try
            {
                HttpClient Http = GenerateHttpClient(language);

                if ((int)Http.PostAsJsonAsync($"{address}{api}", value).Result.StatusCode != 200)
                {
                    throw new Exception("Post failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task DeleteValue(string language, string address, string api, T identifier)
        {
            try
            {
                HttpClient Http = GenerateHttpClient(language);

                StringBuilder sb = new StringBuilder(address).Append(api).Append('/');
                if (typeof(T) == typeof(SavedConfigWrapper))
                {
                    sb.Append((identifier as SavedConfigWrapper).ConfigId).Append('/').Append((identifier as SavedConfigWrapper).SavedName);
                }
                else if (typeof(T) == typeof(ConfigurationDeleteWrapper) || typeof(T) == typeof(ConfigurationDeleteWrapper))
                {
                    sb.Append((identifier as ConfigurationDeleteWrapper).ConfigId);
                }

                int code = (int)Http.DeleteAsync(sb.ToString()).Result.StatusCode;
                if (code != 200)
                {
                    throw new Exception("Deletion failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static HttpClient GenerateHttpClient(string language)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();

                if (!GlobalValues.Secure)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
