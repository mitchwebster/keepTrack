using KeepTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KeepTrack.Api
{
    public class LoginCredentials
    {
        public const int MinPasswordLength = 8;
        public const int MaxUserNameLength = 255;
        public const int MaxPasswordLength = 255;

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        public static void Validate(LoginCredentials credentials)
        {
            if (credentials == null || string.IsNullOrEmpty(credentials.Username))
            {
                throw ApiErrorHelper.CreateResponse(ApiErrorHelper.InvalidUsername);
            }

            if (credentials.Username.Length > MaxUserNameLength)
            {
                throw ApiErrorHelper.CreateResponse(ApiErrorHelper.UsernameTooLong);
            }

            if (string.IsNullOrEmpty(credentials.Password) || credentials.Password.Length < MinPasswordLength)
            {
                throw ApiErrorHelper.CreateResponse(ApiErrorHelper.PasswordDoesNotMeetRequirements, MinPasswordLength);
            }

            if (credentials.Password.Length > MaxPasswordLength)
            {
                throw ApiErrorHelper.CreateResponse(ApiErrorHelper.PasswordTooLong);
            }
        }
    }
}