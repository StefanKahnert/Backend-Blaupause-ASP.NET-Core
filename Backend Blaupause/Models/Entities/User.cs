using Backend_Blaupause.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_Blaupause.Models
{
    public class User : IdentityUser
    {
        public User() { 
            this.UserPermissions = new HashSet<UserPermission>();
        }

        [Column("first_name"), Required]
        public string FirstName { get; set; }

        [Column("last_name"), Required]
        public string LastName { get; set; }

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [Column("password_question")]
        public string PasswordQuestion { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("hidden")]
        public bool Hidden { get; set; }

        [Column("id_adress"), ForeignKey("adress")]
        public long? AdressId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("expiration_date")]
        public DateTime? ExpirationDate { get; set; }

        [Column("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [Column("modified_by")]
        public long? ModifiedBy { get; set; }

        public virtual ICollection<UserPermission> UserPermissions { get; set; }

    }
}
