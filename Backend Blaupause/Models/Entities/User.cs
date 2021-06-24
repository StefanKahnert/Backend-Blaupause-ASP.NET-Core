using Backend_Blaupause.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_Blaupause.Models
{
    public class User
    {
        public User() { 
            this.userPermissions = new HashSet<UserPermission>();
        }

        [Key, Required]
        public long id { get; set; }

        [ForeignKey("supervisor"), Column("supervisor_id")]
        public long? supervisorId { get; set; }

        public virtual User supervisor { get; set; }

        [Required]
        public string username { get; set; }

        [Column("first_name"), Required]
        public string firstName { get; set; }

        [Column("last_name"), Required]
        public string lastName { get; set; }

        public string email { get; set; }

        [Column("last_login")]
        public DateTime? lastLogin { get; set; }

        [JsonIgnore]
        public string password { get; set; }

        [Column("password_question")]
        public string passwordQuestion { get; set; }

        public bool active { get; set; }

        public bool hidden { get; set; }

        [Column("id_adress"), ForeignKey("adress")]
        public long? adressId { get; set; }

        [Column("created_at")]
        public DateTime? createdAt { get; set; }

        [Column("expiration_date")]
        public DateTime? expirationDate { get; set; }

        [Column("modified_at")]
        public DateTime? modifiedAt { get; set; }

        [Column("modified_by")]
        public long? modifiedBy { get; set; }

        public string token { get; set; }

        public virtual ICollection<UserPermission> userPermissions { get; set; }

    }
}
