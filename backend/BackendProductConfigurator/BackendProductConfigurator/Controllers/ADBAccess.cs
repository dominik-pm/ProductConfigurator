using System.Net.Http.Json;

namespace BackendProductConfigurator.Controllers
{
    public static class ADBAccess<T> where T : class
    {
        private static HttpClient Http = new HttpClient();
        public static async Task<List<T>> GetValues(string address, string api)
        {
            return await Http.GetFromJsonAsync<List<T>>($"{address}{api}");
        }
        public static async Task<HttpResponseMessage> PostValue(string address, string api, T value)
        {
            return await Http.PostAsJsonAsync($"{address}{api}", value);
        }
    }
}
