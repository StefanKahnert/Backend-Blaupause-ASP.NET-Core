using Backend_Blaupause.Helper.ExceptionHandling;
using Backend_Blaupause.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Models
{
    public class UserImpl : IUser
    {
        private readonly DatabaseContext _db;

        public UserImpl(DatabaseContext context)
        {
            _db = context;
        }

        public async Task<User> AddUserRecord(User user)
        {
            user.Id = _db.getNextUserId().ToString();
            await _db.User.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task UpdateUserRecord(User User)
        {
            _db.User.Update(User);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUserRecord(string id)
        {
            var entity = await _db.User.FirstOrDefaultAsync(t => t.Id == id);
            _db.User.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<User> GetUserSingleRecord(string id)
        {
            return await _db.User.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<User>> GetUserRecords()
        {
            var result = await _db.User.AsNoTracking().ToListAsync();

            if(result == null || !result.Any())
            {
                throw new HttpException(HttpStatusCode.NoContent);
            }

            return await _db.User.ToListAsync();
        }

        public async Task<UserDTO> getUserDTO(string id)
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

        public async Task<User> getUserByName(string name)
        {
            return await _db.User.AsNoTracking().Include(u => u.UserPermissions).ThenInclude(ur => ur.Permission).FirstOrDefaultAsync(t => t.UserName == name);
        }

    }
}
