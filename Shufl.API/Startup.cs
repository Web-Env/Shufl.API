using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.OpenApi.Models;
using Shufl.API.Infrastructure.Extensions;
using Shufl.API.Infrastructure.Mappers;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Services.Auth;
using Shufl.Domain.Entities;
using System.Collections.Generic;
using WebEnv.Util.Mailer.Settings;

namespace Shufl.API
{
    public class Startup
    {
        private readonly string _corsPolicy = "CorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy, builder => builder
                    .WithOrigins("http://localhost:4200")
                    .WithOrigins("https://localhost:4200")
                    .WithOrigins("https://webenv-shufl.web.app")
                    .WithOrigins("https://www.webenv-shufl.web.app")
                    .WithOrigins("https://shufl.webenv.io")
                    .WithOrigins("https://www.shufl.webenv.io")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true));
            });

            services.AddAsymmetricAuthentication();
            services.AddTransient<AuthenticationService>();

            services.AddDbContext<ShuflContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("ShuflDb")),
                 ServiceLifetime.Scoped);

            var smtpSettingsSection = Configuration.GetSection("SmtpSettings");
            var emailSettings = Configuration.GetSection("EmailSettings");
            var spotifyAPICredentialsSection = Configuration.GetSection("SpotifyAPICredentials");
            services.Configure<SmtpSettings>(smtpSettingsSection);
            services.Configure<EmailSettings>(emailSettings);
            services.Configure<SpotifyAPICredentials>(spotifyAPICredentialsSection);
            services.Configure<AzureFileLoggerOptions>(Configuration.GetSection("AzureLogging"));
            services.AddCustomMappers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shufl", Version = "V1.0.0" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(_corsPolicy);

            app.UseHttpsRedirection();

            app.UseRouting(); 
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                     ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Shufl");
            });
        }

        private ShuflContext ConfigureRepositoryContext()
        {
            var options = new DbContextOptionsBuilder<ShuflContext>()
                .UseSqlServer(Configuration.GetConnectionString("ShuflDb"))
                .Options;
            ShuflContext context = new ShuflContext(options);

            return context;
        }
    }
}
