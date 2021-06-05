using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.DTOs
{
    public class UserDTO
    {
        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public UserDTO() { }

    }
}
