using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConfigurationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
           BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => CreateHostBuilder(args).Build();

        public static IWebHostBuilder CreateBaseHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseAzureAppServices()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = (long)Math.Pow(2, 30);
                })
                .UseStartup<Startup>();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return CreateBaseHostBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                    builder.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json"));
        }
    }
}
