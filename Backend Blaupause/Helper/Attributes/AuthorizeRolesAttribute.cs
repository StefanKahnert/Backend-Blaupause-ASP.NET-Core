using Backend_Blaupause.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Backend_Blaupause.Helper.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Role[] roles) 
        {
            var rolesList = roles.ToList();

            if (!rolesList.Contains(Role.ADMINISTRATOR))
            {
                rolesList.Add(Role.ADMINISTRATOR);
            }

            Roles = string.Join(",", rolesList);
        }
    }
}
