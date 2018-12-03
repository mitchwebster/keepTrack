using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeepTrack.Models
{
    public class Group
    {
        public Group()
        {
            this.Activities = new HashSet<Activity>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual User User { get; set; }
    }
}