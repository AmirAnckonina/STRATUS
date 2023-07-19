namespace StratusApp.Models.MongoDB
{
    public class MyDatabaseSettings
    {
        public MyDatabaseSettings(string connectionString) 
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}
