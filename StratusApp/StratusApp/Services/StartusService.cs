using StratusApp.Services.MongoDBServices;
using Utils.DTO;

namespace StratusApp.Services
{
    public class StratusService : IStratusService
    {
        private readonly MongoDBService _mongoDatabase;

        public StratusService(MongoDBService mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<StratusUser> GetUserByEmail(string email)
        {
            var user = await _mongoDatabase.GetDocuments<StratusUser>(eCollectionName.Users, (user) => user.Email == email);

            return user.FirstOrDefault();
        }

        public async Task<bool> UpdateUserDetails(string userEmail, StratusUser user)
        {
            var dbUsers = await _mongoDatabase.GetDocuments<StratusUser>(eCollectionName.Users, (us) => us.Email == userEmail);
            var dbUser = dbUsers.FirstOrDefault();
            if (dbUser != null)
            {
                user.AccessKey = dbUser.AccessKey; //TODO: use reflection
                user.SecretKey = dbUser.SecretKey;
                user.Region = dbUser.Region;
                user.Password = dbUser.Password;

                await _mongoDatabase.DeleteDocument<StratusUser>(eCollectionName.Users, (us) => us.Email == userEmail);
                await _mongoDatabase.InsertDocument<StratusUser>(eCollectionName.Users, user);

                return true;
            }

            return false;
        }
    }
}
