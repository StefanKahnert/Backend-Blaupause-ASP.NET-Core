﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_Blaupause.Models.Entities
{
    public class UserPermission : IdentityUserRole<string>
    {
        [Column("id_user")]
        public override string UserId { get; set; }

        [Column("id_permission")]
        public override string RoleId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
