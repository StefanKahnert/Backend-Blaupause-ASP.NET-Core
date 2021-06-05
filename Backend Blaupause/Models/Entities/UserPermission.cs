using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.Entities
{
    public class UserPermission
    {
        [Column("id_user")]
        public long userId { get; set; }

        [Column("id_permission")]
        public long permissionId { get; set; }

        [JsonIgnore]
        public virtual User user { get; set; }

        public virtual Permission permission { get; set; }
    }
}
