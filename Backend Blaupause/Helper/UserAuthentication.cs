using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Models;
using Backend_Blaupause.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper
{
    public class UserAuthentication
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUser iUser;

        public UserAuthentication(IHttpContextAccessor httpContext, IUser iUser)
        {
            this.httpContext = httpContext;
            this.iUser = iUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>user Id of current User</returns>
        public long GetUserId()
        {
            try
            {
                var user = httpContext?.HttpContext?.User as ClaimsPrincipal;
                var userId = user.Claims.ElementAt(0).Value;
                return Convert.ToInt64(userId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }
        }

        /// <summary>
        /// checks if user has access to module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="user"></param>
        /// <returns>bool value</returns>
        private bool userHasModule(string module, User user)
        {
            if(module == IUser.NONE || user.userPermissions.Any(up => up.permission.name.Equals(IPermission.ADMINISTRATOR)))
            {
                return true;
            } else
            {
                try
                {
                    return (bool)user.GetType().GetProperty(module).GetValue(user, null);
                }
                catch
                {
                    return false;
                }                
            }
        }

        /// <summary>
        /// checks if user has Permission
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool userHasPermission(string permission, User user)
        {
            if (permission == IPermission.NONE || user.userPermissions.Any(up => up.permission.name.Equals(IPermission.ADMINISTRATOR)))
            {
                return true;
            } else
            {
                return user.userPermissions.Any(r => r.permission.name == permission);
            }
        }

        /// <summary>
        /// checks if user has Permission and Module
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<bool> userHasPermissionAndModule(string permission, string module)
        {
            User user = await iUser.GetUserSingleRecord(GetUserId());
 
            if (userHasPermission(permission, user) && userHasModule(module, user) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// checks if user has permission and ONE of the modules
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public async Task<bool> userHasPermissionAndModules(string permission, List<string> modules)
        {
            User user = await iUser.GetUserSingleRecord(GetUserId());

            if (userHasPermission(permission, user) && modules.Any(module => userHasModule(module, user)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// checks if user has module and ONE of the permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<bool> userHasPermissionsAndModule(List<string> permissions, string module)
        {
            User user = await iUser.GetUserSingleRecord(GetUserId());

            if (permissions.Any(permission => userHasPermission(permission, user)) && userHasModule(module, user))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// checks if user has ONE of the modules and ONE of the permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public async Task<bool> userHasPermissionsAndModules(List<string> permissions, List<string> modules)
        {
            User user = await iUser.GetUserSingleRecord(GetUserId());

            if (permissions.Any(permission => userHasPermission(permission, user)) && modules.Any(module => userHasModule(module, user)))
            {
                return true;
            }

            return false;
        }

        public async Task checkUserIsId(long id)
        {
            User user = await iUser.GetUserSingleRecord(GetUserId());
            
            if(!userHasPermission(IPermission.ADMINISTRATOR, user) && (GetUserId() != id)){
                throw new HttpException(HttpStatusCode.Forbidden, "No access to this record");
            }
        }

        public async Task checkToken(string token)
        {
            if ((await iUser.GetUserSingleRecord(GetUserId())).token != token)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "");
            }
        }
    }
}
