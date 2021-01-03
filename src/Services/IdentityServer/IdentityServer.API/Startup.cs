using System.Reflection;
using IdentityServer.API.Common.Constants;
using IdentityServer.API.Configuration;
using IdentityServer.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Environment = configuration.GetSection("Settings:Environment").Value;
            Configuration = configuration;
            StaticConfiguration = configuration;
        }

        public string Environment { get; }
        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfiguration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(IdentityContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddAppDatabase(connectionString, migrationsAssembly);
            services.AddAppIdentity();
            services.AddAppIdentityServer(connectionString, migrationsAssembly);
            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddApiServices(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment != Environments.Production)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.InitializeConfigurationDatabase();
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}