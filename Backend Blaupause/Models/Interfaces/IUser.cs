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


        void AddUserRecord(User user);
        void UpdateUserRecord(User user);
        void DeleteUserRecord(long id);
        User GetUserSingleRecord(long id);
        User getUserByName(string name);
        List<User> GetUserRecords();
        IQueryable<UserDTO> getUserDTO(long id);

    }
}
