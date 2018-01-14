using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models;

namespace WebAPI.Classes
{
    public class UserWrapper
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public UserWrapper(User user)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.FullName = user.FullName;
            this.Email = user.Email;
        }

        public UserWrapper()
        {
                
        }
    }
}