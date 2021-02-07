using System.Data.Common;
using MercuryBus.Helpers;
using MercuryBus.Local.Kafka.Consumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posting.Infrastructure.Data.Configuration;

namespace Posting.API.Configuration
{
    public static class MercuryBus
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddMercuryBus(this IServiceCollection services)
        {
            services.AddMercuryBusSqlKafkaTransport("mercury", Configuration["Settings:KafkaBoostrapServers"],
                MercuryKafkaConsumerConfigurationProperties.Empty(),
                (serviceProvider, dbContextOptions) =>
                {
                    var connectionProvider = serviceProvider.GetRequiredService<MsSqlConnectionProvider>();
                    dbContextOptions.UseSqlServer((DbConnection) connectionProvider.GetConnection());
                });

            services.AddMercuryBusEventsPublisher();

            return services;
        }
    }
}
