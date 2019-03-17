using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using app.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using ServiceStack;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace app
{
    public static class AppConfigurationExtensions
    {
        public static void AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = services.AddAppSettings(configuration);
            services.AddAppHeaders(appSettings);
            services.AddAppAuth(appSettings);
            services.AddAppDocs(appSettings);
            services.AddAppServices(appSettings);
            services.AddAppDbContext(configuration.GetConnectionString("DefaultConnection"));

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public static void UseAppConfiguration(this IApplicationBuilder app, IHostingEnvironment env)
        {
            var appSettings = app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;

            app.UseForwardedHeaders();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();

            app.UseDefaultFiles();
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
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
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", appSettings.Title);
                c.RoutePrefix = string.Empty;

                c.DocumentTitle = appSettings.Title;
                c.DocExpansion(DocExpansion.None);
            });

            app.UseMvc();

            // run migrations
            using(var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<DataContext>()
                    .Database.Migrate();
            }
        }

        public static AppSettings AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // configure strongly typed settings objects
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            return appSettingsSection.Get<AppSettings>();
        }

        public static void AddAppHeaders(this IServiceCollection services, AppSettings appSettings)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
            }));
        }

        public static void AddAppDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DataContext>(x =>
            {
                // x.UseInMemoryDatabase("TestDb");
                x.UseSqlite(connectionString);
            });
            services.AddAutoMapper();
        }

        public static void AddAppAuth(this IServiceCollection services, AppSettings appSettings)
        {
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                            {
                                var ctx = context;
                                return Task.CompletedTask;
                            },
                            OnChallenge = context =>
                            {
                                var ctx = context;
                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.HttpContext.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/hub")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                var userService = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                                var userId = int.Parse(context.Principal.Identity.Name);
                                var user = userService.GetById(userId);
                                if (user == null)
                                {
                                    // return unauthorized if user no longer exists
                                    context.Fail("Unauthorized");
                                }
                                return Task.CompletedTask;
                            }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        LifetimeValidator = (before, expires, token, param) =>
                            {
                                return expires > DateTime.UtcNow;
                            },
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateActor = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
        }
        public static void AddAppDocs(this IServiceCollection services, AppSettings appSettings)
        {
            // see: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v" + appSettings.MajorVersion, new Info
                {
                    Version = "v" + appSettings.MajorVersion,
                        Title = appSettings.Title,
                        Description = appSettings.Description,
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "Matt Cowan",
                                Email = string.Empty,
                                Url = "https://github.com/mattjcowan"
                        },
                        License = new License
                        {
                            Name = "MIT",
                                Url = "https://github.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxtjs/blob/master/LICENSE"
                        }
                });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                    { { "Bearer", new string[] { } },
                    };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });
        }

        public static void AddAppServices(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
        }
    }

    public class AppSettings
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MajorVersion { get; set; }
        public string Secret { get; set; }
    }

    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}