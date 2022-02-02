using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models.Interfaces
{
    public interface IPermission
    {
        public const string ADMINISTRATOR = "Administrator";
        public const string USER = "User";
        public const string NONE = "None";
    }
}
