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


        Task<User> AddUserRecord(User user);
        Task UpdateUserRecord(User user);
        Task DeleteUserRecord(string id);
        Task<User> GetUserSingleRecord(string id);
        Task<User> getUserByName(string name);
        Task<List<User>> GetUserRecords();
        Task<UserDTO> getUserDTO(string id);

    }
}
