using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeepTrack.Models
{
    public class User
    {
        public User()
        {
            this.Activities = new HashSet<Activity>();
            this.Groups = new HashSet<Group>();
        }

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        public Api.User ToApiObject()
        {
            return new Api.User
            {
                Username = this.Username
            };
        }
    }
}