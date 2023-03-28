using Backend_Blaupause.Enums;
using Backend_Blaupause.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models
{
    public interface IUser
    {
        public const string NONE = "none";
        Task<List<User>> GetAllUsersAsync();
        Task<UserDTO> GetUserDTOByIdAsync(string id);
        Task CreateFakeUsersAsync(int numberOfUsers);
        Task AddRoleToUserAsync(string userId, Role role);
    }
}
