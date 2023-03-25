﻿using Backend_Blaupause.Helper;
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
using System.Text;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUser _user;
        private readonly JWTConfiguration _jwtConfiguration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUser iUser, JWTConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _user = iUser;
            _jwtConfiguration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Login API
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>JWT Token</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AccessToken), (int) HttpStatusCode.OK)]
        public async Task<AccessToken> generateToken([FromBody] UserIdentity credentials)
        {
            string password = SHA512Generator.generateSha512Hash(credentials.Password);

            User user = await _user.getUserByName(credentials.Login);


            if (user == null || user.password != password || user.username != credentials.Login)
            {
                string username = user == null ? "unknown" : user.username;
                _logger.LogInformation("User: " + username + " has failed to login.");
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new AccessToken { Success = false };
            }

            _logger.LogInformation("User: " + user.username + " has successully logged in.");

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            user.userPermissions.ToList().ForEach(up =>
            {
                claims.Add(new Claim(ClaimTypes.Role, up.permission.name));
            });

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfiguration.SecretKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiredOn = DateTime.Now.AddSeconds(_jwtConfiguration.TokenExpirationTime);

            JwtSecurityToken token = new JwtSecurityToken(_jwtConfiguration.ValidIssuer,
                  _jwtConfiguration.ValidAudience,
                  claims,
                  expires: expiredOn,
                  signingCredentials: creds);


            string tokenHash = new JwtSecurityTokenHandler().WriteToken(token);

            await _user.UpdateUserRecord(user);

            return new AccessToken
            {
                ExpireOnDate = token.ValidTo,
                Success = true,
                ExpiryIn = _jwtConfiguration.TokenExpirationTime,
                Token = tokenHash
            };
    }

    }
}
