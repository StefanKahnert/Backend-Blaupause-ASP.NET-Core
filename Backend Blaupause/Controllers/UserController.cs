﻿using Backend_Blaupause.Helper;
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
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> getUserById(long id)
        {
            if(id <= 0)
            {
                return BadRequest($"{nameof(id)} must be equal or larger than 0");
            }

            await _userAuthentication.CheckUserIsIdAsync(id);

            return Ok(await iUser.GetUserSingleRecord(id));
        }

        [HttpGet, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await iUser.GetUserRecords());
        }

        [HttpPost, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> addUser(User user)
        {
            return Ok(await iUser.AddUserRecord(user));
        }

        [HttpGet]
        [Route("dto/{id:int}"), APILog]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpException), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IQueryable<UserDTO>>> getUserDTO(long id)
        {
            await _userAuthentication.CheckUserIsIdAsync(id);

            return Ok(await iUser.getUserDTO(id));
        }
    }
}
