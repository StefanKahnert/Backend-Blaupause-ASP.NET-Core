using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.Entities
{
    [Table("permission")]
    public class Permission
    {
        public Permission() { this.userPermissions = new HashSet<UserPermission>(); }

        [Key, Required]
        public long id { get; set; }

        public string name { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserPermission> userPermissions { get; set; }
    }
}
