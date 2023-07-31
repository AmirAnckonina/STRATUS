using StratusApp.Models;
using StratusApp.Services.MongoDBServices;
using Utils.DTO;
using MongoDB.Driver;
using BCrypt.Net;
using System;
using System.Security.Cryptography;
using System.Text;

namespace StratusApp.Services
{
    public class AuthService
    {
        private readonly MongoDBService _mongoDatabase;

        public AuthService(MongoDBService mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public Task<bool> IsUserExists(StratusUser user)
        {
            bool isUserExists = false;
            List<StratusUser> dbUsers = _mongoDatabase.GetCollectionAsList<StratusUser>(eCollectionName.Users).Result;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if(dbUsers.Any(dbUser => dbUser.Username.Equals(user.Username)) || dbUsers.Any(dbUser => dbUser.Password.Equals(hashedPassword)) 
                || dbUsers.Any(dbUser => dbUser.Email.Equals(user.Email)))
            {
                isUserExists = true;
            }
            else
            {
                isUserExists = false;
            }
            return Task.FromResult(isUserExists);
        }

        public void InsertUserToDB(StratusUser user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            string hashedAccessKey = EncryptionHelpers.EncryptionHelper.Encrypt(user.AccessKey);
            string hashedSecretKey = EncryptionHelpers.EncryptionHelper.Encrypt(user.SecretKey);

            user.Password = hashedPassword;
            user.AccessKey = hashedAccessKey;
            user.SecretKey = hashedSecretKey;
            _mongoDatabase.InsertDocument<StratusUser>(eCollectionName.Users, user);
        }
    }
}
