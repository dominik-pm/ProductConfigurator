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
                return new StringBuilder("http").Append((Secure) ? "s://" : "://").Append(serverAddress).Append(':').Append((Secure) ? Ports[0] : Ports[1]).ToString();
            }
            set { serverAddress = value; }
        }
        private static string serverAddress;
        public static bool Secure { get; set; }
        public static int[] Ports { get; set; }
        public static int MinutesBetweenFetches { get; set; }
        public static string EmailServer { get; set; }
        public static string ImagesFolder { get; set; }
        public static string PDFOutput { get; set; }
    }
}
