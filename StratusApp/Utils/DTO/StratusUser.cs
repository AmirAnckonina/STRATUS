using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace StratusApp.Models
{
    public class StratusUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("accesKey")]
        public string AccessKey { get; set; }
        [BsonElement("secretKey")]
        public string SecretKey { get; set; }
        public StratusUser(string username, string email, string password, string accessKey, string secretKey)
        {
            Username = username;
            Email = email;
            Password = password;
            AccessKey = accessKey;
            SecretKey = secretKey;
        }
    }
}