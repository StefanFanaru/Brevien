using System.Linq;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Posting.Core.Helpers;
using Posting.Core.Interfaces.Asp;

namespace Posting.API.Configuration
{
    public static class CommandAndQueries
    {
        public static CommandAndQueriesComposition AddHandlers(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            foreach (var validatorType in assemblies.Select(x => x.GetTypes())
                .SelectMany(Generics.DerivedOf(typeof(IValidator<>))))
            {
                services.AddTransient(validatorType.ClosedGenericType, validatorType.Type);
            }

            return new CommandAndQueriesComposition(services);
        }
    }

    public class CommandAndQueriesComposition
    {
        private readonly IServiceCollection _services;

        public CommandAndQueriesComposition(IServiceCollection services)
        {
            _services = services;
        }

        public IServiceCollection WithPipelineValidation()
        {
            foreach (var service in Generics.DerivedOf(typeof(IRequestHandler<,>))(_services.Select(x => x.ServiceType).ToList()))
            {
                var resultEntry = Generics.MatchForOpenGenerics(service.Param2, typeof(IOperationResult<>));

                if (resultEntry != null)
                {
                    _services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(service.Param1, service.Param2),
                        typeof(ValidationBehaviour<,>).MakeGenericType(service.Param1, resultEntry.Param1));
                }
            }

            return _services;
        }
    }
}