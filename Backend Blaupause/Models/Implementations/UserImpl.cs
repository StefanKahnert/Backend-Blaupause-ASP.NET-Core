using Backend_Blaupause.Enums;
using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Helper.Factories;
using Backend_Blaupause.Models.DTOs;
using Backend_Blaupause.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models
{
    public class UserImpl : IUser
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Permission> _roleManager;
        private readonly ILogger<UserImpl> _logger;

        public UserImpl(DatabaseContext context, UserManager<User> userManager, ILogger<UserImpl> logger, RoleManager<Permission> roleManager)
        {
            _db = context;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task DeleteUserRecord(string id)
        {
            var entity = await _db.User.FirstOrDefaultAsync(t => t.Id == id);
            _db.User.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var result = await _db.User.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();

            if (result == null || !result.Any())
            {
                throw new HttpException(HttpStatusCode.NoContent);
            }

            return result;
        }

        public async Task<UserDTO> GetUserDTOByIdAsync(string id)
        {
            var userDTO = from u in _db.User.AsNoTracking()
                          where u.Id == id
                          select new UserDTO()
                          {
                              Id = u.Id,
                              FirstName = u.FirstName,
                              LastName = u.LastName
                          };
            return await userDTO.FirstOrDefaultAsync();
        }

        public async Task CreateFakeUsersAsync(int numberOfUsers)
        {
            var fakeUsers = UserFactory.GenerateUsers(numberOfUsers);

            foreach(var fakeUser in fakeUsers)
            {
                var userExists = await _userManager.FindByNameAsync(fakeUser.UserName);
                if (userExists != null)
                {
                    continue;
                }

                var result = await _userManager.CreateAsync(fakeUser, "1234ABDsds@");
                if (!result.Succeeded)
                {
                    _logger.LogError($"Creation of User: {fakeUser.UserName} failed!");
                }
            }
        }

        public async Task AddRoleToUserAsync(string userId, Role role)
        {
            if(!(await _roleManager.RoleExistsAsync(role.ToString())))
            {
                await _roleManager.CreateAsync(new Permission(role));
            }

            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.AddToRoleAsync(user, role.ToString());

            if (!result.Succeeded)
            {
                throw new HttpException(HttpStatusCode.InternalServerError, $"Failed to add Role: {role} to User: {userId}");
            }
        }
    }
}
