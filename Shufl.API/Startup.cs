using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shufl.API.Settings;

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

            var smtpSettingsSection = Configuration.GetSection("SmtpSettings");
            var spotifyAPICredentialsSection = Configuration.GetSection("SpotifyAPICredentials");
            services.Configure<SmtpSettings>(smtpSettingsSection);
            services.Configure<SpotifyAPICredentials>(spotifyAPICredentialsSection);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shufl", Version = "V1.0.0" });
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
    }
}
