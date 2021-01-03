using System.Linq;
using System.Reflection;
using Blog.API.Asp;
using Blog.API.Infrastructure.Data;
using Blog.API.Infrastructure.Data.Migrations;
using Blog.API.Services;
using Blog.API.Services.Interfaces;
using FluentValidation;
using Madara.Api.Composition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.API.Configuration
{
    public static class ApiServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoDb"));
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddScoped<IDataMigration, M001_TestMigration>();
            services.AddScoped<IDataMigrator, DataMigrator>();
            services.AddScoped<MongoDbClient>();

            return services;
        }

        public static IServiceCollection AddFluentValidators(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var validatorType in assemblies.Select(x => x.GetTypes())
                .SelectMany(Generics.DerivedOf(typeof(IValidator<>))))
            {
                services.AddTransient(validatorType.ClosedGenericType, validatorType.Type);
            }

            return services;
        }
    }
}
