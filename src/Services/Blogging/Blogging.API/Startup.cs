using Blogging.API.Asp;
using Blogging.API.Asp.Validators;
using Blogging.API.Configuration;
using Blogging.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blogging.API
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
        .AddSwaggerConfiguration()
        .AddFluentValidators(typeof(BlogControllerValidators.CreateValidator).Assembly);

      services.AddControllers().AddApplicationPart(typeof(Startup).Assembly).AddNewtonsoftJson(options =>
      {
        options.AllowInputFormatterExceptionMessages = true;
        options.SerializerSettings.Converters.Add(new JsonExtensions.UtcDateTimeConverter());
        options.SerializerSettings.Converters.Add(new JsonExtensions.TrimmingStringConverter());
      });

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

      app.UseRouting();
      ConfigureAuth(app);
      app.AddSwagger(pathBase);
      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}