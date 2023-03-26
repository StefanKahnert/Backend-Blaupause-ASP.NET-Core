using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_Blaupause.Models.Entities
{
    public class UserPermission : IdentityUserRole<string>
    {
        [JsonIgnore]
        public virtual User User { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
