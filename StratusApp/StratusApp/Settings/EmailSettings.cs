namespace StratusApp.Settings
{
    public class EmailSettings
    {
        public EmailSettings(string apiKey)
        {
            ApiKey = apiKey;
        }

        public EmailSettings() { }

        public string ApiKey { get; set; }
    }
}
