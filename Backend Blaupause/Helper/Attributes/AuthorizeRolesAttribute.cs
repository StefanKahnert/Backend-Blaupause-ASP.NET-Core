using Backend_Blaupause.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Backend_Blaupause.Helper.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) 
        {
            var rolesList = roles.ToList();

            if (!rolesList.Contains(IPermission.ADMINISTRATOR))
            {
                rolesList.Add(IPermission.ADMINISTRATOR);
            }

            Roles = string.Join(",", rolesList);
        }
    }
}
