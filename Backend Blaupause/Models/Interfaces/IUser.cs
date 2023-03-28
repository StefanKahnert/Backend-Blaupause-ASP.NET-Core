using Backend_Blaupause.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models
{
    public interface IUser
    {
        public const string NONE = "none";
        Task<List<User>> GetUserRecords();
        Task<UserDTO> getUserDTO(string id);
        Task CreateFakeUsers(int numberOfUsers);
    }
}
