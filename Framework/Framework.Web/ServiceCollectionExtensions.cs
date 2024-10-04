using EventStore.Client;
using Framework.Core;
using Framework.Impl.EventStore;
using Framework.Impl.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Framework.Web;

public static class ServiceCollectionExtensions
{
    public static void RegisterHandlers(this IServiceCollection services, Assembly assembly)
    {
        // Register Event Handlers
        var eventHandlerType = typeof(IEventHandler<>);
        RegisterGenericHandlers(services, assembly, eventHandlerType);
    }

    private static void RegisterGenericHandlers(IServiceCollection services, Assembly assembly, Type handlerType)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface) // Exclude abstract classes and interfaces
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
            .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == handlerType)
            .ToList();

        foreach (var type in types)
        {
            services.AddSingleton(type.Interface, type.Type); // Register each handler as a singleton
        }
    }

    public static IServiceCollection AddEventListenerBackgroundService(this IServiceCollection services, string eventPrefix = "")
    {
        services.AddHostedService(provider =>
        {
            var eventListener = provider.GetRequiredService<IEventListener>();
            var eventStore = provider.GetRequiredService<IEventStore>();

            // Create the EventListenerBackgroundService with the eventPrefix
            return new EventListenerBackgroundService(eventListener, eventStore, eventPrefix);
        });

        return services;
    }

    public static IServiceCollection AddEventStore(this IServiceCollection services, string? eventStoreDbConnectionString = null)
    {
        if (!string.IsNullOrEmpty(eventStoreDbConnectionString))
        {
            var settings = EventStoreClientSettings.Create(eventStoreDbConnectionString);
            var eventStoreClient = new EventStoreClient(settings);
            services.AddSingleton<IEventStore>(new EventStoreDb(eventStoreClient));
        }
        else
        {
            // Optionally add an in-memory event store if no connection string is provided
            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        return services;
    }
}
