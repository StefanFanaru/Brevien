using Blog.API.Asp;
using Blog.API.Asp.Validators;
using Blog.API.Configuration;
using Blog.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Blog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfiguration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfiguration { get; private set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApiServices()
                .AddFluentValidators(typeof(BlogControllerValidators.CreateValidator).Assembly);

            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly).AddNewtonsoftJson(options =>
            {
                options.AllowInputFormatterExceptionMessages = true;
                options.SerializerSettings.Converters.Add(new JsonExtensions.UtcDateTimeConverter());
                options.SerializerSettings.Converters.Add(new JsonExtensions.TrimmingStringConverter());
            });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Blog.API", Version = "v1"}); });

            services.AddAuth();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRequestResponseLogging();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "Blog.API V1"); });

            app.UseHttpsRedirection();

            app.UseRouting();

            ConfigureAuth(app);

            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization("ApiScope"); });
        }
    }
}
