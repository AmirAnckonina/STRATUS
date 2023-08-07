using Amazon.EC2;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Utils.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient.AwsServices.AwsUtils
{
    public class EC2ClientFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConcurrentDictionary<string, AmazonEC2Client> _clients;
        private readonly ConcurrentDictionary<string, Dictionary<eAWSCredentials, string>> _credentials;

        public EC2ClientFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _clients = new ConcurrentDictionary<string, AmazonEC2Client>();
            _credentials = new ConcurrentDictionary<string, Dictionary<eAWSCredentials, string>>();
        }
        public bool StoreAWSCredentialsInSession(string accessKey, string secretKey)
        {
            try
            {
                lock (_credentials)
                {
                    var sessionId = _httpContextAccessor.HttpContext.Session.Id;
                    // Store the session ID in a cookie
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("Stratus", sessionId, new CookieOptions
                    {
                        HttpOnly = true, // Optional: Ensures the cookie is not accessible from JavaScript
                        SameSite = SameSiteMode.Lax, // Adjust this according to your requirements
                                                     // Other cookie options as needed
                    });
                    _credentials[sessionId] = new Dictionary<eAWSCredentials, string>
                    {
                        { eAWSCredentials.AccessKey, accessKey },
                        { eAWSCredentials.SecretKey, secretKey }
                    };
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public Dictionary<eAWSCredentials, string> GetAWSCredentialsFromSession()
        {
            var sessionId = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];

            lock (_credentials)
            {
                if (_credentials.TryGetValue(sessionId, out var credentials))
                {
                    return credentials;
                }
                else
                {
                    return new Dictionary<eAWSCredentials, string>();
                }
            }
        }
        public AmazonEC2Client GetAndCreateEC2ClientIfNotExist()
        {
            var sessionId = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];

            lock (_clients)
            {
                // Check if the EC2Client is already stored for the user's session
                if (_clients.TryGetValue(sessionId, out var existingClient))
                {
                    return existingClient;
                }
                else
                {
                    // If EC2Client doesn't exist for the session, create a new one and store it
                    var credentials = new BasicAWSCredentials(GetAWSCredentialsFromSession()[eAWSCredentials.AccessKey], GetAWSCredentialsFromSession()[eAWSCredentials.SecretKey]);
                    //var region = RegionEndpoint.USWest2; // Replace with your desired region
                    var ec2Client = new AmazonEC2Client(credentials);

                    // Store the EC2Client in the dictionary with the user's session ID as the key
                    _clients[sessionId] = ec2Client;

                    return ec2Client;
                }
            }
        }
    }
}
