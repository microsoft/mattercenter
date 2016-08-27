using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Legal.MatterCenter.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Service
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if(!context.Request.Headers.Keys.Contains("Authorization"))
            {
                var response = new ErrorResponse()
                {
                    Message = "UnAuthorized",                    
                    Description = "The request object doesnot contain any authorization header",
                    ErrorCode = "401",
                    IsTokenValid = false
                };
                string message = JsonConvert.SerializeObject(response);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(message);
                return;
            }
            string authorization = context.Request.Headers["Authorization"];
            if(!ValidateToken(authorization))
            {
                var response = new ErrorResponse()
                {
                    Message = "UnAuthorized",
                    Description = "The authorization header contains invalid token",
                    ErrorCode = "401",
                    IsTokenValid = false
                };
                string message = JsonConvert.SerializeObject(response);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(message);
                return;
            }
            await _next.Invoke(context);
        }

        private bool ValidateToken(string accessToken)
        {
            var authBits = accessToken.Split(' ');
            if (authBits.Length != 2)
            {                
                return false;
            }
            if (!authBits[0].ToLowerInvariant().Equals("bearer"))
            {                
                return false;
            }
            return true;
        }        
    }
}
