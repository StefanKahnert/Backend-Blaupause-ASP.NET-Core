using Backend_Blaupause.Helper;
using Backend_Blaupause.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthenticationEndpoint : ControllerBase
    {
        private readonly IUser iUser;
        private readonly JWTConfiguration configuration;
        private readonly ILogger<AuthenticationEndpoint> logger;

        public AuthenticationEndpoint(IUser iUser, JWTConfiguration configuration, ILogger<AuthenticationEndpoint> logger)
        {
            this.iUser = iUser;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Login API
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>JWT Token</returns>
        [HttpPost]
        public AccessToken generateToken([FromBody] UserIdentity credentials)
        {
            string password = string.Empty;
            password = SHA512Generator.generateSha512Hash(credentials.Password);

            User user = iUser.getUserByName(credentials.Login);

            if (user == null || user.password != password || user.username != credentials.Login)
            {
                string username = user == null ? "unknown" : user.username;
                logger.LogInformation("User: " + username + " has failed to login.");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new AccessToken { Success = false };
            } else
            {
                logger.LogInformation("User: " + user.username + " has successully logged in.");
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.SecretKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiredOn = DateTime.Now.AddSeconds(configuration.TokenExpirationTime);

            JwtSecurityToken token = new JwtSecurityToken(configuration.ValidIssuer,
                  configuration.ValidAudience,
                  claims,
                  expires: expiredOn,
                  signingCredentials: creds);


            return new AccessToken
            {
                ExpireOnDate = token.ValidTo,
                Success = true,
                ExpiryIn = configuration.TokenExpirationTime,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

    }
}
