using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using KeepTrack.Models;
using KeepTrack.Helpers;
using KeepTrack.App_Start;

namespace KeepTrack.Controllers
{
    public class UsersController : ApiController
    {
        [Route("users")]
        [HttpPost]
        public async Task<Api.User> CreateUser(Api.LoginCredentials credentials)
        {
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                Api.LoginCredentials.Validate(credentials);
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(credentials.Username, StringComparison.OrdinalIgnoreCase));
                if (existingUser != null)
                {
                    throw ApiErrorHelper.CreateResponse(ApiErrorHelper.UserAlreadyExists, credentials.Username);
                }

                var newUser = new User
                {
                    Username = credentials.Username,
                    PasswordHash = await LoginHelper.GenerateHash(credentials.Password),
                    Role = RoleType.User.ToString()
                };

                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
                return newUser.ToApiObject();
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<string> RequestToken(Api.LoginCredentials credentials)
        {
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                Api.LoginCredentials.Validate(credentials);
                var desiredUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(credentials.Username, StringComparison.OrdinalIgnoreCase));
                if (desiredUser == null || !(await LoginHelper.VerifyHash(credentials.Password, desiredUser.PasswordHash)))
                {
                    throw ApiErrorHelper.CreateResponse(ApiErrorHelper.UserNotFound, credentials.Username);
                }

                return TokenHelper.GenerateToken(desiredUser, credentials.Password);
            }
        }

        [Route("users")]
        [HttpGet]
        [TokenAuth(MinimumAllowedRoleType = RoleType.Admin)]
        public async Task<IEnumerable<Api.User>> GetUsers()
        {
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                var users = await dbContext.Users.ToListAsync();
                return users.Select(u => u.ToApiObject());
            }
        }
    }
}
