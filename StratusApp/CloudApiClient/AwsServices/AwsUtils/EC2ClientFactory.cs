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
using Amazon;
using Utils.Utils;


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
        public bool StoreAWSCredentialsInSession(string email, string accessKey, string secretKey, string region)
        {
            try
            {
                lock (_credentials)
                {
                    var sessionId = email;
                    // Store the session ID in a cookie
                    //_httpContextAccessor.HttpContext.Response.Cookies.Append("Stratus", sessionId, new CookieOptions
                    //{
                    //    HttpOnly = true, // Optional: Ensures the cookie is not accessible from JavaScript
                    //    SameSite = SameSiteMode.Lax, // Adjust this according to your requirements
                    //                                 // Other cookie options as needed
                    //});
                    _credentials[sessionId] = new Dictionary<eAWSCredentials, string>
                    {
                        { eAWSCredentials.AccessKey, accessKey },
                        { eAWSCredentials.SecretKey, secretKey },
                        { eAWSCredentials.Region, region }
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
            var sessionId = SessionUtils.GetSessionId(_httpContextAccessor);

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
            lock (_clients)
            {
                var sessionId = SessionUtils.GetSessionId(_httpContextAccessor);
                // Check if the EC2Client is already stored for the user's session
                if (_clients.TryGetValue(sessionId, out var existingClient))
                {
                    return existingClient;
                }
                else
                {
                    // If EC2Client doesn't exist for the session, create a new one and store it
                    var credentials = new BasicAWSCredentials(GetAWSCredentialsFromSession()[eAWSCredentials.AccessKey], GetAWSCredentialsFromSession()[eAWSCredentials.SecretKey]);
                    var region = RegionEndpoint.GetBySystemName(GetAWSCredentialsFromSession()[eAWSCredentials.Region]); // Replace with your desired region
                    var ec2Client = new AmazonEC2Client(credentials, region);

                    // Store the EC2Client in the dictionary with the user's session ID as the key
                    _clients[sessionId] = ec2Client;

                    return ec2Client;
                }
            }
        }
    }
}
      