using Backend_Blaupause.Helper;
using Backend_Blaupause.Models.DTOs;
using Backend_Blaupause.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await db.user.FirstOrDefaultAsync(t => t.id == id);
        }

        public async Task<List<User>> GetUserRecords()
        {
            return await db.user.ToListAsync();
        }

        public async Task<IQueryable<UserDTO>> getUserDTO(long id)
        {
            var userDTO = from u in db.user
                          where u.id == id
                              //join u2 in db.User on u.id equals u2.id
                          select new UserDTO()
                          {
                              id = u.id,
                              firstName = u.firstName,
                              lastName = u.lastName
                          };
            return userDTO;
        }

        public async Task<User> getUserByName(string name)
        {
            return await db.user.Include(u => u.userPermissions).ThenInclude(ur => ur.permission).FirstOrDefaultAsync(t => t.username == name);
        }

    }
}
