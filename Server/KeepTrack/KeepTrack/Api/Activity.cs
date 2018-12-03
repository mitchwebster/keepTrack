using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KeepTrack.Api
{
    [DataContract]
    public class Activity
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string MeasurementType { get; set; }

        [DataMember]
        public string Group { get; set; }
    }
}