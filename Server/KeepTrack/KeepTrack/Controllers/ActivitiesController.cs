using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KeepTrack.Models;
using KeepTrack.App_Start;
using KeepTrack.Helpers;

namespace KeepTrack.Controllers
{
    public class ActivitiesController : ApiController
    {
        [Route("activities")]
        [HttpGet]
        [TokenAuth(MinimumAllowedRoleType = RoleType.User)]
        public async Task<IEnumerable<Api.Activity>> GetActivities(int? groupId = null)
        {
            var username = this.User.Identity.Name;
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                var activities = !groupId.HasValue ? await dbContext.Activities.Where(a => a.User.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).ToListAsync() :
                    await dbContext.Activities.Where(a => a.User.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && a.GroupId == groupId).ToListAsync();
                return activities.Select(a => a.ToApiObject());
            }
        }

        [Route("activities")]
        [HttpPost, HttpPut]
        [TokenAuth(MinimumAllowedRoleType = RoleType.User)]
        public async Task<Api.Activity> CreateOrUpdateActivity(Api.Activity newApiActivity)
        {
            var userName = this.User.Identity.Name;
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                var desiredUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (desiredUser == null)
                {
                    throw ApiErrorHelper.CreateResponse(ApiErrorHelper.UserNotFound, userName);
                }

                Group group = null;
                if (!string.IsNullOrEmpty(newApiActivity.Group))
                {
                    group = desiredUser.Groups.FirstOrDefault(g => g.Name.Equals(newApiActivity.Group, StringComparison.OrdinalIgnoreCase));
                    if (group == null)
                    {
                        throw ApiErrorHelper.CreateResponse(ApiErrorHelper.GroupNotFound, newApiActivity.Group);
                    }
                }

                var newActivity = new Activity
                {
                    Name = newApiActivity.Name,
                    Category = newApiActivity.Category,
                    MeasurementType = newApiActivity.MeasurementType,
                    Group = group,
                    User = desiredUser
                };

                var existingActivity = desiredUser.Activities.FirstOrDefault(a => a.Name.Equals(newApiActivity.Name, StringComparison.OrdinalIgnoreCase));
                if (existingActivity == null)
                {

                    dbContext.Activities.Add(newActivity);
                }
                else
                {

                    existingActivity.Update(newActivity);
                }

                await dbContext.SaveChangesAsync();
                return newActivity.ToApiObject();
            }
        }
    }
}