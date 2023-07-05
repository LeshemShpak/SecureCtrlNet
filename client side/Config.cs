

namespace fin_pro
{
    public static class Config
    {
        public const int port_1 = 6060;
        public const int port_2 = 6066;
        public const int port_3 = 6666;
        // Get the user's home directory
        public static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\RPC";

    }
}
