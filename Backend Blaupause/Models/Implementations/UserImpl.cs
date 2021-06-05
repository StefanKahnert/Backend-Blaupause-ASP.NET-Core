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

        public void AddUserRecord(User user)
        {
            user.id = db.getNextUserId();
            db.user.Add(user);
            db.SaveChanges();
        }

        public void UpdateUserRecord(User User)
        {
            db.user.Update(User);
            db.SaveChanges();
        }

        public void DeleteUserRecord(long id)
        {
            var entity = db.user.FirstOrDefault(t => t.id == id);
            db.user.Remove(entity);
            db.SaveChanges();
        }

        public User GetUserSingleRecord(long id)
        {
            return db.user.FirstOrDefault(t => t.id == id);
        }

        public List<User> GetUserRecords()
        {
            return db.user.ToList();
        }

        public IQueryable<UserDTO> getUserDTO(long id)
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

        public User getUserByName(string name)
        {
            return db.user.Include(u => u.userPermissions).ThenInclude(ur => ur.permission).FirstOrDefault(t => t.username == name);
        }

    }
}
