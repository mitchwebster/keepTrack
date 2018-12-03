using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace KeepTrack.Helpers
{
    public class ApiErrorHelper
    {
        // User errors
        public static readonly ErrorResponse InvalidUsername = new ErrorResponse { StatusCode = HttpStatusCode.BadRequest, MessageFormat = "Username is a required field." };
        public static readonly ErrorResponse UsernameTooLong = new ErrorResponse { StatusCode = HttpStatusCode.BadRequest, MessageFormat = "Username is too long. Please reduce the size." };
        public static readonly ErrorResponse PasswordTooLong = new ErrorResponse { StatusCode = HttpStatusCode.BadRequest, MessageFormat = "Password is too long. Please reduce the size." };
        public static readonly ErrorResponse PasswordDoesNotMeetRequirements = new ErrorResponse { StatusCode = HttpStatusCode.BadRequest, MessageFormat = "Password does not meet requirements. Please ensure it contains at least {0} characters." };
        public static readonly ErrorResponse UserNotFound = new ErrorResponse { StatusCode = HttpStatusCode.NotFound, MessageFormat = "User {0} was not found." };
        public static readonly ErrorResponse UserAlreadyExists = new ErrorResponse { StatusCode = HttpStatusCode.Conflict, MessageFormat = "User {0} already exists. Please try a different username." };

        // Groups
        public static readonly ErrorResponse GroupNotFound = new ErrorResponse { StatusCode = HttpStatusCode.NotFound, MessageFormat = "Group {0} was not found." };

        public static HttpResponseException CreateResponse(ErrorResponse err, params object[] args)
        {
            var response = new HttpResponseMessage(err.StatusCode)
            {
                Content = new StringContent(string.Format(err.MessageFormat, args))
            };

            return new HttpResponseException(response);
        }

        public class ErrorResponse
        {
            public HttpStatusCode StatusCode { get; set; }
            public string MessageFormat { get; set; }
        }
    }
}