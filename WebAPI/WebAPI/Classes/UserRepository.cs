using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Classes;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI
{
    public class UserRepository
    {
        internal static Dictionary<int, User> Users = new Dictionary<int, User>();
        private static Random random = new Random();

        public List<User> GetAll()
        {
            return Users.Values.ToList();
        }

        public User Get(int id)
        {
            if (!Users.ContainsKey(id))
                throw new NotFoundException("Can't find user");

            return Users[id];
        }

        public User Get(string username)
        {
            User user = Users.Values.FirstOrDefault(u => string.Equals(u.Username, username));

            if (user == null)
                throw new NotFoundException("Can't find user");

            return user;
        }

        public User Create(User user)
        {
            if (user.Id == 0)
                user.Id = Users.Keys.Count;

            if (Users.Values.Any(u => u.Username == user.Username))
                throw new ConflictException("Username already exists");

                user.AuthToken = RandomString(8);

            Users[user.Id] = user;

            return user;
        }


        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<User> Get(int page, int size)
        {
            return Users
                .Select(kvp => kvp.Value)
                .OrderBy(s => s.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public void Update(User user)
        {
            if (!Users.ContainsKey(user.Id))
                throw new NotFoundException("Can't find user");

            Users[user.Id] = user;
        }

        public UserWrapper Update(int id, UserUpdateRequest editedUser)
        {
            if (!Users.ContainsKey(id))
                throw new NotFoundException("Can't find user");

            Users[id].UpdateUser(editedUser);

            return new UserWrapper(Users[id]);
        }

        public void Delete(int id)
        {
            if (!Users.ContainsKey(id))
                throw new NotFoundException("Can't find user");

            Users.Remove(id);
        }

        public void AddBet(int id)
        {
            if (!Users.ContainsKey(id))
                throw new NotFoundException("Can't find user");


        }
    }
}