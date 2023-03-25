using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_Blaupause.Models.Entities
{
    public class Permission : IdentityRole
    {
        public Permission() { this.userPermissions = new HashSet<UserPermission>(); }

        [JsonIgnore]
        public virtual ICollection<UserPermission> userPermissions { get; set; }
    }
}
