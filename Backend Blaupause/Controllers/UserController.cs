using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.Attributes;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.DTOs;
using Backend_Blaupause.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;

        public UserController(IUser userModel)
        {
            _user = userModel;
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<User>> getUserById(long id)
        {
            if(id <= 0)
            {
                return BadRequest($"{nameof(id)} must be equal or larger than 0");
            }

            return Ok(await _user.GetUserSingleRecord(id));
        }

        [HttpGet]
        [AuthorizeRoles(IPermission.USER, IPermission.ADMINISTRATOR)]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await _user.GetUserRecords());
        }

        [HttpPost]
        [AuthorizeRoles(IPermission.ADMINISTRATOR)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<User>> addUser(User user)
        {
            return Ok(await _user.AddUserRecord(user));
        }

        [HttpGet]
        [Route("dto/{id:int}"), APILog]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserDTO>> getUserDTO(long id)
        {
            return await _user.getUserDTO(id);
        }
    }
}
