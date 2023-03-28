using Backend_Blaupause.Enums;
using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.Attributes;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly UserManager<User> _userManager;

        public UserController(IUser user, UserManager<User> userManager)
        {
            _user = user;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<User>> getUserById(string id)
        {
            return Ok(await _userManager.FindByIdAsync(id));
        }

        [HttpGet]
        [AuthorizeRoles(Role.USER)]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await _user.GetUserRecords());
        }

        [HttpGet]
        [Route("dto/{id}"), APILog]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserDTO>> getUserDTO(string id)
        {
            return await _user.getUserDTO(id);
        }

        [HttpGet]
        [Route("me"), APILog]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<User>> GetMyUserDataAsync()
        {
            var userName = HttpContext.User?.Identity?.Name;

            return Ok(await _userManager.FindByNameAsync(userName));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("fake/{number}"), APILog]
        public async Task<ActionResult> getUserDTO(int number)
        {
            await _user.CreateFakeUsers(number);

            return Ok($"{number} Users created");
        }
    }
}
