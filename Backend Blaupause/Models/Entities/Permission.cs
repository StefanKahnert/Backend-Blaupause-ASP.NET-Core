using Backend_Blaupause.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_Blaupause.Models.Entities
{
    public class Permission : IdentityRole
    {
        public Permission() { UserPermissions = new HashSet<UserPermission>(); }

        public Permission(Role role) : base(role.ToString())
        {
            UserPermissions = new HashSet<UserPermission>();
        }

        [JsonIgnore]
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
