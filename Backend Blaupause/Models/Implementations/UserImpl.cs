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
        private readonly DatabaseContext db;

        public UserImpl(DatabaseContext context)
        {
            db = context;
        }

        public async Task<User> AddUserRecord(User user)
        {
            user.id = db.getNextUserId();
            await db.user.AddAsync(user);
            await db.SaveChangesAsync();

            return user;
        }

        public async Task UpdateUserRecord(User User)
        {
            db.user.Update(User);
            await db.SaveChangesAsync();
        }

        public async Task DeleteUserRecord(long id)
        {
            var entity = await db.user.FirstOrDefaultAsync(t => t.id == id);
            db.user.Remove(entity);
            await db.SaveChangesAsync();
        }

        public async Task<User> GetUserSingleRecord(long id)
        {
            return await db.user.AsNoTracking().FirstOrDefaultAsync(t => t.id == id);
        }

        public async Task<List<User>> GetUserRecords()
        {
            var result = await db.user.AsNoTracking().ToListAsync();

            if(result == null || !result.Any())
            {
                throw new HttpException(HttpStatusCode.NoContent);
            }

            return await db.user.ToListAsync();
        }

        public async Task<UserDTO> getUserDTO(long id)
        {
            var userDTO = from u in db.user.AsNoTracking()
                          where u.id == id
                          select new UserDTO()
                          {
                              id = u.id,
                              firstName = u.firstName,
                              lastName = u.lastName
                          };
            return await userDTO.FirstOrDefaultAsync();
        }

        public async Task<User> getUserByName(string name)
        {
            return await db.user.AsNoTracking().Include(u => u.userPermissions).ThenInclude(ur => ur.permission).FirstOrDefaultAsync(t => t.username == name);
        }

    }
}
