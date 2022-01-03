using System.Net.Http.Json;

namespace DBAccess
{
    public static class DBAccess<T> where T : class
    {
        public static async Task<T> GetValues(string api)
        {
            HttpClient Http = new HttpClient();
            return await Http.GetFromJsonAsync<T>($"https://localhost:7109/{api}");
        }
    }
}