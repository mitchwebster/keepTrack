using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using System.Threading.Tasks;
using KeepTrack.Models;

namespace KeepTrack.Helpers
{
    public class TokenHelper
    {
        private const string Secret = "";
        private const int DefaultExpiryTimeInMinutes = 20;

        public static string GenerateToken(User user, string password, int expireMinutes = DefaultExpiryTimeInMinutes)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(stoken);
        }

        public static async Task<ClaimsIdentity> AuthenticateJwtToken(string token)
        {
            var simplePrinciple = TokenHelper.GetPrincipal(token);
            if (simplePrinciple == null)
            {
                return null;
            }

            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return null;
            }

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            var roleClaim = identity.FindFirst(ClaimTypes.Role);
            var username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var dbRoleType = RoleType.User;
            using (var dbContext = DbContextFactory.GetDbContext())
            {
                var desiredUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (desiredUser == null)
                {
                    return null;
                }

                if (!RoleType.TryParse(desiredUser.Role, out dbRoleType))
                {
                    return null;
                }
            }

            var role = roleClaim?.Value;
            if (!RoleType.TryParse(role, out RoleType roleType))
            {
                return null;
            }

            // Validate that this user is still allowed to use this token
            if (dbRoleType < roleType)
            {
                return null;
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, roleType.ToString())
                };

            return new ClaimsIdentity(claims, "Jwt");
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public enum RoleType
    {
        Blocked,
        User,
        Admin
    }
}