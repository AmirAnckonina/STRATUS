using MongoDB.Bson;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Utils.DTO;

namespace StratusApp.Models.MongoDB
{
    public class AlertDocument : BsonDocument
    {
        public string MahcineId { get; set; }
        public eAlertType Type { get; set; }
        public DateTime CreationTime { get; set; }
        public double PercentageUsage { get; set; }

        public AlertDocument()
        {
            var properties = typeof(AlertDocument).GetProperties().Where(p => p.DeclaringType == typeof(AlertDocument));

            foreach (PropertyInfo property in properties)
            {
                base.Add(property.Name, BsonTypeMapper.MapToBsonValue(property.GetValue(this)));
            }
        }
    }
}
