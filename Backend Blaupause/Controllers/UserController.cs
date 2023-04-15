using Backend_Blaupause.Enums;
using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.Attributes;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
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

        /// <summary>
        /// Get User by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<User>> GetUserByIdAsync(string id)
        {
            return Ok(await _userManager.FindByIdAsync(id));
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeRoles(Role.ADMINISTRATOR)]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<User>>> GetAllAsync([FromQuery] PaginationParameter paginationParamter)
        {
            return Ok(await _user.GetAllUsersAsync(paginationParamter.PageNumber, paginationParamter.PageSize));
        }

        /// <summary>
        /// Get User-DTO by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("dto/{id}"), APILog]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserDTO>> GetUserDtoByIdAsync(string id)
        {
            return await _user.GetUserDTOByIdAsync(id);
        }


        /// <summary>
        /// Get current User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me"), APILog]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<User>> GetMyUserDataAsync()
        {
            var userName = HttpContext.User?.Identity?.Name;

            return Ok(await _userManager.FindByNameAsync(userName));
        }

        /// <summary>
        /// Create amount of Fake User
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("fake/{number}"), APILog]
        public async Task<ActionResult> CreateFakeUsersAsync(int number)
        {
            await _user.CreateFakeUsersAsync(number);

            return Ok($"{number} Users created");
        }

        /// <summary>
        /// Add Role To User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("{userId}/role/{role}")]
        public async Task<ActionResult> AddRoleToUserAsync(string userId, Role role)
        {
            await _user.AddRoleToUserAsync(userId, role);

            return Ok($"Role has been given to user");
        }
    }
}
