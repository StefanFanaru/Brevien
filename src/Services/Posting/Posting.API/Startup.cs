using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Posting.API.Asp;
using Posting.API.Configuration;
using Posting.Infrastructure.Commands;
using Posting.Infrastructure.Helpers;
using Posting.Infrastructure.Operations;

namespace Posting.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfiguration = configuration;
            Environment = configuration.GetSection("Settings:Environment").Value;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfiguration { get; private set; }
        public string Environment { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiServices();
            services.AddFluentMigrations();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Posting.API", Version = "v1"}); });

            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly).AddNewtonsoftJson(options =>
            {
                options.AllowInputFormatterExceptionMessages = true;
                options.SerializerSettings.Converters.Add(new JsonExtensions.UtcDateTimeConverter());
                options.SerializerSettings.Converters.Add(new JsonExtensions.TrimmingStringConverter());
                options.SerializerSettings.Converters.Add(new OperationResultConverter());
            });

            // services.AddAuth();
            services.AddHandlers(typeof(CreatePostCommand).Assembly).WithPipelineValidation();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            // app.UseAuthentication();
            // app.UseAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Posting.API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            ConfigureAuth(app);

            // app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization("ApiScope"); });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
