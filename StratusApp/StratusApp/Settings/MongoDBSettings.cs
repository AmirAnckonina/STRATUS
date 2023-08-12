namespace StratusApp.Settings
{
    public class MongoDBSettings
    {
        public MongoDBSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public MongoDBSettings() { }

        public string ConnectionString { get; set; }
    }
}
