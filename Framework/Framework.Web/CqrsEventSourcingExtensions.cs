using Framework.Core;
using Framework.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Web;

public static class CqrsEventSourcingExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Type aggregateType)
    {
        var aggregateAssembly = aggregateType.Assembly;

        services.AddMediatR(cfg =>
        {
            // Register all handlers from the assembly where your command/query handlers are defined
            cfg.RegisterServicesFromAssembly(aggregateAssembly);
        });

        // Register command and query handlers
        services.RegisterHandlers(aggregateAssembly);

        // Define the repository type you want to register dynamically
        var repositoryType = typeof(AggregateRepository<>).MakeGenericType(aggregateType);

        // Register the AggregateRepository with the services
        services.AddSingleton(repositoryType);

        // Resolve the IReadRepository and IReadModel automatically
        var readModelType = aggregateAssembly.GetTypes()
            .FirstOrDefault(t => typeof(IReadModel).IsAssignableFrom(t));

        if (readModelType != null)
        {
            // Assume the repository follows the convention <Model>ReadRepository
            var readRepositoryType = aggregateAssembly.GetTypes()
                .FirstOrDefault(t => typeof(IReadRepository<>).MakeGenericType(readModelType).IsAssignableFrom(t));

            if (readRepositoryType != null)
            {
                // Register the repository
                services.AddSingleton(
                    typeof(IReadRepository<>).MakeGenericType(readModelType),
                    readRepositoryType);
            }

            // Add Event Listener for the read model
            services.AddEventListener(readModelType.Assembly);
        }
        else
        {
            Console.WriteLine("Cannot find implementation for IReadModel");
        }

        return services;
    }

    public static IServiceCollection WithEvents(this IServiceCollection services, string eventPrefix = "")
    {
        // If an event prefix is provided, register the background service
        if (!string.IsNullOrEmpty(eventPrefix))
        {
            services.AddEventListenerBackgroundService(eventPrefix);
        }
        else
        {
            // Register the background service without a prefix
            services.AddEventListenerBackgroundService();
        }

        return services;
    }
}
