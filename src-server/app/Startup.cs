using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using app.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppConfiguration(Configuration);

            services.AddSignalR();

            services.AddNodeServices();
            services.AddScoped<IScreenshotTaker, ScreenshotTaker>();
            services.AddScoped<IBookmarksRepository, BookmarksRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAppConfiguration(env);

            var uploadsDir = Path.Combine(env.ContentRootPath, "App_Data", "Uploads");
            Directory.CreateDirectory(uploadsDir);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsDir),
                    RequestPath = "/uploads"
            });
            app.UseSignalR(routeBuilder =>
            {
                routeBuilder.MapHub<AppHub>("/hub");
            });

            app.Run(async(context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}