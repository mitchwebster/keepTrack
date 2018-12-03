using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeepTrack.Models
{
    public class Activity
    {
        public Activity()
        {
            this.Entries = new HashSet<Entry>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string Category { get; set; }
        public string MeasurementType { get; set; }

        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Entry> Entries { get; set; }

        public Api.Activity ToApiObject()
        {
            return new Api.Activity
            {
                Name = this.Name,
                Category = this.Category,
                MeasurementType = this.MeasurementType,
                Group = Group != null ? Group.Name : string.Empty
            };
        }

        public bool IsForUser(string username)
        {
            return this.User.Username.Equals(username, StringComparison.OrdinalIgnoreCase);
        }

        public void Update(Activity activity)
        {
            this.Category = activity.Category;
            this.MeasurementType = activity.MeasurementType;
            this.Group = activity.Group;
        }
    }
}