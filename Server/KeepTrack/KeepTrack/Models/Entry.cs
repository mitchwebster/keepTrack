using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeepTrack.Models
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }
        public DateTime EntryTimeStamp { get; set; }
        public double Value { get; set; }
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }
    }
}