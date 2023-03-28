using Backend_Blaupause.Enums;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly JWTConfiguration _jwtConfiguration;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Permission> _roleManager;

        public AuthenticationController(JWTConfiguration configuration, ILogger<AuthenticationController> logger, UserManager<User> userManager, RoleManager<Permission> roleManager)
        {
            _jwtConfiguration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Login API
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>JWT Token</returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(AccessToken), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<AccessToken>> Login([FromBody] LoginModel credentials)
        {
            User user = await _userManager.FindByNameAsync(credentials.Login);

            var passwordValid = await _userManager.CheckPasswordAsync(user, credentials.Password);

            if (user == null || !passwordValid)
            {
                string username = user == null ? "unknown" : user.UserName;
                _logger.LogInformation("User: " + username + " has failed to login.");
                return Unauthorized();
            }

            _logger.LogInformation("User: " + user.UserName + " has successully logged in.");

            var authClaims = await GenerateClaims(user);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
            SigningCredentials creds = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
            DateTime expiredOn = DateTime.Now.AddSeconds(_jwtConfiguration.TokenExpirationTime);

            JwtSecurityToken token = new JwtSecurityToken(_jwtConfiguration.ValidIssuer,
                  _jwtConfiguration.ValidAudience,
                  authClaims,
                  expires: expiredOn,
                  signingCredentials: creds);


            string tokenHash = new JwtSecurityTokenHandler().WriteToken(token);

            await _userManager.UpdateSecurityStampAsync(user);

            return new AccessToken
            {
                ExpireOnDate = token.ValidTo,
                Success = true,
                ExpiryIn = _jwtConfiguration.TokenExpirationTime,
                Token = tokenHash
            };
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterModel model)
        {
            var result = await CreateUser(model);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            } else
            {
                return Ok(result);
            }
        }

        /// <summary>
        /// Create Admin User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult<User>> RegisterAdmin([FromBody] RegisterModel model)
        {
            var result = await CreateUser(model);

            if(result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            if (!await _roleManager.RoleExistsAsync(Role.ADMINISTRATOR.ToString()))
            {
                await _roleManager.CreateAsync(new Permission(Role.ADMINISTRATOR));
            }

            await _userManager.AddToRoleAsync(result, Role.ADMINISTRATOR.ToString());

            return result;
        }

        private async Task<User> CreateUser(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return null;
            }

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return null;
            } else
            {
                return user;
            }
        }

        private async Task<List<Claim>> GenerateClaims(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return authClaims;
        }

    }
}
