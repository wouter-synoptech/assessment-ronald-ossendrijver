namespace ParcelHandling.Server
{
    public static class AppSettings
    {
        public static IConfiguration? Configuration { get; set; }

        public static string? Get(string settingName) => Configuration?[settingName];
    }
}
