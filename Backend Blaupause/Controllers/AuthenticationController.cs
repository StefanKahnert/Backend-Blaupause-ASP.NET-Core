using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUser iUser;
        private readonly JWTConfiguration configuration;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IUser iUser, JWTConfiguration configuration, ILogger<AuthenticationController> logger)
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
        public async Task<AccessToken> generateToken([FromBody] UserIdentity credentials)
        {
            string password = SHA512Generator.generateSha512Hash(credentials.Password);

            User user = await iUser.getUserByName(credentials.Login);


            if (user == null || user.password != password || user.username != credentials.Login)
            {
                string username = user == null ? "unknown" : user.username;
                logger.LogInformation("User: " + username + " has failed to login.");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new AccessToken { Success = false };
            }

            if (!string.IsNullOrEmpty(user.token))
            {
				string tokenString = user.token.Replace("Bearer ", "");
				JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
				DateTime tokenExpiryDate = (handler.ReadToken(tokenString) as JwtSecurityToken).ValidTo;

				//No Force Login and Token not expired
				if (!credentials.forceLogin && tokenExpiryDate > DateTime.Now)
				{
					throw new HttpException(HttpStatusCode.Conflict, "There is already a login session!");
				}
			}

            logger.LogInformation("User: " + user.username + " has successully logged in.");

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


            string tokenHash = new JwtSecurityTokenHandler().WriteToken(token);

            user.token = "Bearer " + tokenHash;

            await iUser.UpdateUserRecord(user);

            return new AccessToken
            {
                ExpireOnDate = token.ValidTo,
                Success = true,
                ExpiryIn = configuration.TokenExpirationTime,
                Token = tokenHash
            };
    }

    }
}
