using System;
using ConfigurationService.Data;
using ConfigurationService.Exctensions;
using ConfigurationService.Models;
using ConfigurationService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConfigurationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseClient();
            services.TryAddScoped<IDocumentRepository<Device, Guid>>(sl =>
            {
                var databaseId = sl.GetService<IConfiguration>().GetValue<string>("DatabaseId");
                return new CosmosDBRepository<Device, Guid>(sl.GetService<IDocumentClient>(), databaseId);
            });
            services.TryAddScoped<IFileRepository, FileRepository>();
            services.TryAddSingleton<IStorageClientProvider, StorageClientProvider>();
            services.TryAddTransient<IDeviceService<Guid>, DeviceService>();
            services.TryAddTransient<IDeviceAttachmentsService, DeviceAttachmentService>();
            services.TryAddTransient<IFolderStructureProvider, FolderStructureProvider>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDeveloperUser();
            }

            app.InitializeDatabaseAsync().GetAwaiter().GetResult();
            app.UseMvc();
        }
    }
}
