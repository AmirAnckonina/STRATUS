namespace StratusApp.Models.MongoDB
{
    public class MyDatabaseSettings
    {
        public MyDatabaseSettings(string connectionString) 
        {
            this.ConnectionString = connectionString;
        }

        public MyDatabaseSettings() { }

        public string ConnectionString { get; set; }
    }
}
