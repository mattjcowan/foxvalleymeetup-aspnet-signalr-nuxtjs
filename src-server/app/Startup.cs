using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace app
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => { builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); }));

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseSignalR(routeBuilder =>
            {
                routeBuilder.MapHub<AppHub>("/hub");
            });

            app.UseDefaultFiles();
            //app.UseStaticFiles();
            app.UseStaticFiles(GetStaticFileOptions() as StaticFileOptions);

            app.Run(async(context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private StaticFileOptions GetStaticFileOptions()
        {
            return new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    if (ctx.Context.Response.ContentType.Contains("text/html", StringComparison.OrdinalIgnoreCase) ||
                        headers.ContentType.MediaType.Value.Contains("text/html", StringComparison.OrdinalIgnoreCase))
                    {
                        headers.CacheControl = new CacheControlHeaderValue
                        {
                            NoStore = true,
                            NoCache = true,
                            Public = true
                            // MaxAge = TimeSpan.FromDays(0)
                        };
                    }
                }
            };
        }
    }
}