using System;
using System.Collections.Generic;
using System.Text;

namespace ReturnPolicy.Infrastructure.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}