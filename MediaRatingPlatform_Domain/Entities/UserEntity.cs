using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.Entities
{
    public class UserEntity
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isActive { get; set; } = false;
        public DateTime createdAt { get; set; }
        public DateTime lastLoginAt { get; set; }

        public UserEntity(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
