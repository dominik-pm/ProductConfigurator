using Model.Enumerators;
using System.Text;

namespace BackendProductConfigurator.App_Code
{
    public static class GlobalValues
    {
        public static EValueMode ValueMode { get; set; }
        public static string ServerAddress
        {
            get
            {
                return new StringBuilder(serverAddress).Append(':').Append((Secure) ? Ports[0] : Ports[1]).ToString();
            }
            set { serverAddress = value; }
        }
        private static string serverAddress;
        public static bool Secure { get; set; }
        public static int[] Ports { get; set; } = { 7129, 5129 };
        public static int MinutesBetweenFetches { get; set; }
    }
}
