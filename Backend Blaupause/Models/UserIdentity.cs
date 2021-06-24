using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models
{
    public class UserIdentity
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public bool forceLogin { get; set; }
    }
}
