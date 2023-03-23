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
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUser _user;

        public UserAuthentication(IHttpContextAccessor httpContext, IUser user)
        {
            _httpContext = httpContext;
            _user = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>user Id of current User</returns>
        public long GetUserId()
        {
            try
            {
                var user = _httpContext?.HttpContext?.User;
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
        private bool UserHasModule(string module, User user)
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
        public bool UserHasPermission(string permission, User user)
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
        public async Task<bool> UserHasPermissionAndModuleAsync(string permission, string module)
        {
            User user = await _user.GetUserSingleRecord(GetUserId());
 
            if (UserHasPermission(permission, user) && UserHasModule(module, user) )
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
        public async Task<bool> UserHasPermissionAndModulesAsync(string permission, List<string> modules)
        {
            User user = await _user.GetUserSingleRecord(GetUserId());

            if (UserHasPermission(permission, user) && modules.Any(module => UserHasModule(module, user)))
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
        public async Task<bool> UserHasPermissionsAndModuleAsync(List<string> permissions, string module)
        {
            User user = await _user.GetUserSingleRecord(GetUserId());

            if (permissions.Any(permission => UserHasPermission(permission, user)) && UserHasModule(module, user))
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
        public async Task<bool> UserHasPermissionsAndModulesAsync(List<string> permissions, List<string> modules)
        {
            User user = await _user.GetUserSingleRecord(GetUserId());

            if (permissions.Any(permission => UserHasPermission(permission, user)) && modules.Any(module => UserHasModule(module, user)))
            {
                return true;
            }

            return false;
        }

        public async Task CheckUserIsIdAsync(long id)
        {
            User user = await _user.GetUserSingleRecord(GetUserId());
            
            if(!UserHasPermission(IPermission.ADMINISTRATOR, user) && (GetUserId() != id)){
                throw new HttpException(HttpStatusCode.Forbidden, "No access to this record");
            }
        }

        public async Task CheckTokenAsync(string token)
        {
            if ((await _user.GetUserSingleRecord(GetUserId())).token != token)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "");
            }
        }
    }
}
