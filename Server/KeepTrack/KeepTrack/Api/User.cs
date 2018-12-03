using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KeepTrack.Api
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Username { get; set; }
    }
}