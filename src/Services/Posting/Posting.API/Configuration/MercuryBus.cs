using System.Data.SqlClient;
using MercuryBus.Consumer.Common;
using MercuryBus.Events.Subscriber;
using MercuryBus.Helpers;
using MercuryBus.Local.Kafka.Consumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posting.API.Events;
using Posting.Core.Interfaces.Data;

namespace Posting.API.Configuration
{
    public static class MercuryBus
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddMercuryBus(this IServiceCollection services)
        {
            services.AddSingleton<IMessageHandlerDecorator, MercuryExecutionStrategyMessageHandlerDecorator>();
            services.AddScoped<IDomainEventHandler<BlogCreatedEvent>, BlogCreatedEventHandler>();

            services.AddMercuryBusSqlKafkaTransport("mercury", Configuration["Settings:KafkaBoostrapServers"],
                MercuryKafkaConsumerConfigurationProperties.Empty(),
                (serviceProvider, dbContextOptions) =>
                {
                    var connectionProvider = serviceProvider.GetRequiredService<IDbConnectionProvider>();
                    dbContextOptions.UseSqlServer(new SqlConnection(Configuration["ConnectionString"]));
                });

            services.AddMercuryBusEventsPublisher();

            services.AddMercuryBusDomainEventDispatcher("posting-api",
                provider => DomainEventHandlersBuilder.ForAggregateType("Blog")
                    .OnEvent<BlogCreatedEvent, IDomainEventHandler<BlogCreatedEvent>>()
                    .Build());

            return services;
        }
    }
}