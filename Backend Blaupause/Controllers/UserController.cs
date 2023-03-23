using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.DTOs;
using Backend_Blaupause.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUser iUser;

        private readonly UserAuthentication _userAuthentication;

        public UserController(IUser userModel, UserAuthentication userAuthentication)
        {
            this.iUser = userModel;
            this._userAuthentication = userAuthentication;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<User>> getUserById(long id)
        {
            await _userAuthentication.CheckUserIsIdAsync(id);

            return Ok(await iUser.GetUserSingleRecord(id));
        }

        [HttpGet, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await iUser.GetUserRecords());
        }

        [HttpPost, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        public async Task<IActionResult> addUser(User user)
        {
            return Ok(await iUser.AddUserRecord(user));
        }

        [HttpGet]
        [Route("dto/{id:int}"), APILog]
        public async Task<ActionResult<IQueryable<UserDTO>>> getUserDTO(long id)
        {
            await _userAuthentication.CheckUserIsIdAsync(id);

            return Ok(await iUser.getUserDTO(id));
        }
    }
}
