using System;
using System.Transactions;
using MercuryBus.Consumer.Common;
using Microsoft.Extensions.DependencyInjection;
using Posting.Core.Interfaces.Data;

namespace Posting.API.Configuration
{
    public class MercuryExecutionStrategyMessageHandlerDecorator : IMessageHandlerDecorator, IOrdered
    {
        public Action<SubscriberIdAndMessage, IServiceProvider, IMessageHandlerDecoratorChain> Accept =>
            async (subscriberIdAndMessage, serviceProvider, chain) =>
            {
                var dbConnectionProvider = serviceProvider.GetRequiredService<IDbConnectionProvider>();
                using var connection = dbConnectionProvider.GetConnection();
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                chain.InvokeNext(subscriberIdAndMessage, serviceProvider);
                scope.Complete();
            };

        public int Order => BuiltInMessageHandlerDecoratorOrder.DuplicateDetectingMessageHandlerDecorator - 1;
    }
}