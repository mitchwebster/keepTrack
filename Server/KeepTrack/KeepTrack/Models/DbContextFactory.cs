using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeepTrack.Models
{
    public class DbContextFactory
    {
        private static string ConnectionString = "";

        public static KTContext GetDbContext()
        {
            return new KTContext(ConnectionString);
        }
    }
}