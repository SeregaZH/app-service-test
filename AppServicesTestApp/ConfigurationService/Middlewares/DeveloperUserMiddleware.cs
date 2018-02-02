using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigurationService.Middlewares
{
    public sealed class DeveloperUserMiddleware
    {
        private readonly RequestDelegate _next;

        public DeveloperUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Country, "Belarus"),
                new Claim(ClaimTypes.GivenName, "Ivan Ivanov")
            };
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return _next(context);
        }
    }
}
