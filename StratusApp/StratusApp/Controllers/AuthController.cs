using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using StratusApp.Services;
using StratusApp.Services.EncryptionHelpers;
using Utils.DTO;
using StratusApp.Settings;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class AuthController : Controller
    {
        private readonly MongoDBService _mongoDatabase;
        private readonly AuthService _authService;
        private readonly string userDosentExists = "User doesn't exists";

        public AuthController(MongoDBService mongoDatabase, AppSettings appSettings)
        {
            _mongoDatabase = mongoDatabase;
            _authService = new AuthService(mongoDatabase, appSettings);
        }

        [HttpGet("RegisterToStratusService")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> RegisterToStratusService(string username, string email, string password, string accessKey,string secretKey)
        {
            var registerToStratusServiceResp = new StratusResponse<StratusUser>();
            StratusUser user = new StratusUser(
                username,
                email,
                password,
                accessKey,
                secretKey
                );
            
            string isUserExistsMessage = await _authService.IsUserExists(user);
            if (isUserExistsMessage.Equals(userDosentExists))
            {
                _authService.InsertUserToDB(user);
                registerToStratusServiceResp.Data = user;
                registerToStratusServiceResp.Message = "Registered successfully";
                return Ok(registerToStratusServiceResp);

            }
            else
            {
                registerToStratusServiceResp.Message = isUserExistsMessage;
                return BadRequest(registerToStratusServiceResp);
            }
        }
        [HttpGet("LogInToStratusService")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> LogInToStratusService(string email, string password)
        {
            var logInToStratusServiceResp = new StratusResponse<StratusUser>();
            if (await _authService.IsUserRegistered(email, password) == true)
            {
                var user = await _authService.GetUserFromDB(email); 
                logInToStratusServiceResp.Data = user;
                logInToStratusServiceResp.Message = "Logged in successfully";
                return Ok(logInToStratusServiceResp);
            }
            else
            {
                logInToStratusServiceResp.Message = "Email or Password are incorrect, please try again";
                return BadRequest(logInToStratusServiceResp);
            }
        }


        [HttpGet("GetDocumentByFilter")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetDocumentByFilter(eCollectionName collectionType, string fieldName, string value)
        {
            var getDocumentByFilterResp = new StratusResponse<StratusUser>();
            //var filterdDocs = await _mongoDatabase.GetDocuments(collectionType, (documnet) => documnet.GetValue(fieldName).AsString == value);

            return Ok();
        }
        [HttpGet("GenerateRandomKey")]
        public async Task<ActionResult<StratusResponse<string>>> GenerateRandomKey()
        {
            var generateRandomKeyResp = new StratusResponse<string>();
            var randomKey = KeyGenerator.GetBase64EncodedKey(KeyGenerator.GenerateRandomKey(32));
            generateRandomKeyResp.Data = randomKey;
            return Ok(generateRandomKeyResp);
        }

    }
}
