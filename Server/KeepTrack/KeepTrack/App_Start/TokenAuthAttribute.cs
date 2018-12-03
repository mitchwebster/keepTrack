using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using KeepTrack.Helpers;

namespace KeepTrack.App_Start
{
    public class TokenAuthAttribute : Attribute, IAuthenticationFilter
    {
        public RoleType MinimumAllowedRoleType { get; set; }

        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new TokenAuthFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var claimsIdentity = await TokenHelper.AuthenticateJwtToken(token);

            if (claimsIdentity == null)
            {
                context.ErrorResult = new TokenAuthFailureResult("Invalid token", request);
                return;
            }

            var roleClaim = claimsIdentity.Claims.First(c => c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));
            RoleType roleType;
            if (!RoleType.TryParse(roleClaim.Value, out roleType))
            {
                context.ErrorResult = new TokenAuthFailureResult("Invalid permissions", request);
                return;
            }

            if (roleType < MinimumAllowedRoleType)
            {
                context.ErrorResult = new TokenAuthFailureResult("Invalid permissions", request);
                return;
            }

            context.Principal = new ClaimsPrincipal(claimsIdentity);
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // TODO: figure this out later
        }
    }
}