namespace StratusApp.Settings
{
    public class AppSettings
    {
        public AppSettings() { }

        public MongoDBSettings MongoDBSettings { get; set; }
        public EncryptionSettings EncryptionSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
