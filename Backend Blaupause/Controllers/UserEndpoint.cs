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
    [Route("user")]
    public class UserEndpoint : ControllerBase
    {
        private readonly IUser iUser;

        private readonly UserAuthentication userAuthentication;

        public UserEndpoint(IUser userModel, UserAuthentication userAuthentication)
        {
            this.iUser = userModel;
            this.userAuthentication = userAuthentication;
        }

        [HttpGet]
        [Route("{id:int}")]
        public User getUserById(long id)
        {
            userAuthentication.checkUserIsId(id);

            return iUser.GetUserSingleRecord(id);
        }

        [HttpGet, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        public IEnumerable<User> Get()
        {

            throw new HttpException(HttpStatusCode.Forbidden, "Nicht erlaubt");
            return iUser.GetUserRecords().ToList();
        }

        [HttpPost, Permission(IPermission.ADMINISTRATOR, IUser.NONE)]
        public void addUser(User user)
        {
            iUser.AddUserRecord(user);
        }

        [HttpGet]
        [Route("dto/{id:int}"), APILog]
        public IQueryable<UserDTO> getUserDTO(long id)
        {
            userAuthentication.checkUserIsId(id);

            return iUser.getUserDTO(id);
        }
    }
}
