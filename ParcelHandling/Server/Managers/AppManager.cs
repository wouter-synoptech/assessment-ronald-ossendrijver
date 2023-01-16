namespace ParcelHandling.Server.Managers
{
    public static class AppManager
    {
        public static string GetConfiguredPath(IConfiguration? configuration, string settingName)
        {
            if (configuration == null)
            {
                throw new ArgumentException($"Configuration of {settingName} missing");
            }
            else
            {
                var configuredSetting = configuration[settingName];
                if (configuredSetting == null)
                {
                    throw new ArgumentException("Container folder not configured");
                }
                else
                {
                    return Path.Combine(AppContext.BaseDirectory, configuredSetting);
                }
            }
        }
    }
}
