using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backend_Blaupause.Models.Interfaces;
using System.Net;
using Backend_Blaupause.Helper.ExceptionHandling;
using System.Diagnostics.CodeAnalysis;

namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PermissionAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// checks if User has Permission and access to Module
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="module"></param>
        public PermissionAttribute([NotNull]string permission, [NotNull]string module) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission, module };
        }

        /// <summary>
        /// checks if User has ONE of the Permissions and access to Module
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="module"></param>
        public PermissionAttribute([NotNull] List<string> permissions, [NotNull] string module) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissions, module };
        }

        /// <summary>
        /// checks if User has Permission and access to ONE of the Modules
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="module"></param>
        public PermissionAttribute([NotNull] string permission, [NotNull] List<string> modules) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission, modules };
        }

        /// <summary>
        /// checks if User has ONE of the Permissions and access to ONE of the Modules
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="module"></param>
        public PermissionAttribute([NotNull] List<string> permissions, [NotNull] List<string> modules) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissions, modules };
        }

        private class PermissionFilter : IActionFilter
        {
            private readonly UserAuthentication userAuthentication;

            private string permission { get; set; }
            private string module { get; set; }
            private List<string> permissions { get; set; }
            private List<string> modules { get; set; }

            public PermissionFilter(UserAuthentication userAuthentication, string permission, string module)
            {
                this.userAuthentication = userAuthentication;
                this.permission = permission;
                this.module = module;
            }

            public PermissionFilter(UserAuthentication userAuthentication, string permission, List<string> modules)
            {
                this.userAuthentication = userAuthentication;
                this.permission = permission;
                this.modules = modules;
            }

            public PermissionFilter(UserAuthentication userAuthentication, List<string> permissions, string module)
            {
                this.userAuthentication = userAuthentication;
                this.module = module;
                this.permissions = permissions;
            }
            public PermissionFilter(UserAuthentication userAuthentication, List<string> permissions, List<string> modules)
            {
                this.userAuthentication = userAuthentication;
                this.permissions = permissions;
                this.modules = modules;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if(this.permission != null && this.module != null)
                {
                    if (!userAuthentication.userHasPermissionAndModule(this.permission, this.module))
                    {
                        throw new HttpException(HttpStatusCode.Forbidden, "Sie benötigen die Berechtigung '" + this.permission + "' und Zugriff auf Modul " + this.module);
                    }
                } else if (this.permission != null && this.modules != null)
                {
                    if (!userAuthentication.userHasPermissionAndModules(this.permission, this.modules))
                    {
                        throw new HttpException(HttpStatusCode.Forbidden, "Sie benötigen die Berechtigung '" + this.permission + "' und Zugriff auf eines der Module " + this.modules);
                    }
                } else if (this.permissions != null && this.module != null)
                {
                    if (!userAuthentication.userHasPermissionsAndModule(this.permissions, this.module))
                    {
                        throw new HttpException(HttpStatusCode.Forbidden, "Sie benötigen die Berechtigung '" + this.permissions + "' und Zugriff auf eines der Module " + this.module);
                    }
                } else if (this.permissions != null && this.modules != null)
                {
                    if (!userAuthentication.userHasPermissionsAndModules(this.permissions, this.modules))
                    {
                        throw new HttpException(HttpStatusCode.Forbidden, "Sie benötigen die Berechtigung '" + this.permissions + "' und Zugriff auf eines der Module " + this.modules);
                    }
                }


            }
        }

    }
}
