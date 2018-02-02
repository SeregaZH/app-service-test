using ConfigurationService.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace ConfigurationService.Exctensions
{
    public static class DeveloperTools
    {
        public static IApplicationBuilder UseDeveloperUser(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseMiddleware<DeveloperUserMiddleware>();
        }
    }
}
