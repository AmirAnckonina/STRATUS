using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.MongoDB;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using System;
using Utils.DTO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using Amazon.Runtime.Documents;
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

        internal Task<bool> IsUserExists(string username, string password)
        {
            throw new NotImplementedException();
        }

        internal Task RegisterToStratusService(string username, string password, string accessKey, string secretKey)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return null;
        }

    }
}
