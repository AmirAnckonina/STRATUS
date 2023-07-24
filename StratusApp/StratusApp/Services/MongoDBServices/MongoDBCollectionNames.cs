namespace StratusApp.Services.MongoDBServices
{
    public class MongoDBCollectionNames
    {
        public static string? GetCollectionName(eCollectionName collectionName)
        {
            return Enum.GetName(typeof(eCollectionName), collectionName);
        }
    }


    public enum eCollectionName
    {
        Alerts,
        AlertConfigurations,
        AlternativeInstances,
        Users,
        Instances,
    }
}