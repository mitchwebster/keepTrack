using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace KeepTrack.Models
{
    public class KTContext : DbContext
    {
        public KTContext(string connectionStr) : base(connectionStr)
        {
        }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Entry> Entries { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}