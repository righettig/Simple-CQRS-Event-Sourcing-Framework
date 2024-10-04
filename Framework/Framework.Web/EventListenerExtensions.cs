using Framework.Core;
using Framework.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Framework.Web;

public static class EventListenerExtensions
{
    public static IServiceCollection AddEventListener<TReadModel>(
        this IServiceCollection services,
        Assembly eventAssembly,
        Assembly? handlerAssembly = null) where TReadModel : class
    {
        handlerAssembly ??= eventAssembly;

        services.AddSingleton<IEventListener, EventListener<TReadModel>>(provider =>
        {
            var readRepository = provider.GetRequiredService<IReadRepository<TReadModel>>();
            var eventListener = new EventListener<TReadModel>(readRepository);

            // Register events from eventAssembly and handlers from handlerAssembly
            RegisterEventHandlers(eventListener, eventAssembly, handlerAssembly);

            return eventListener;
        });

        return services;
    }

    public static IServiceCollection AddEventListener(
        this IServiceCollection services,
        Assembly assembly)
    {
        var readModelType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IReadModel).IsAssignableFrom(t) && !t.IsAbstract);

        if (readModelType == null)
        {
            throw new InvalidOperationException("No IReadModel found in the provided assembly.");
        }

        var readRepositoryType = typeof(IReadRepository<>).MakeGenericType(readModelType);
        var eventListenerType = typeof(EventListener<>).MakeGenericType(readModelType);

        services.AddSingleton(typeof(IEventListener), provider =>
        {
            var readRepository = provider.GetRequiredService(readRepositoryType);
            var eventListener = Activator.CreateInstance(eventListenerType, readRepository);

            // Register events and handlers in the same assembly
            RegisterEventHandlers(eventListener, assembly, assembly);

            return eventListener;
        });

        return services;
    }

    private static void RegisterEventHandlers(
        object eventListener,
        Assembly eventAssembly,
        Assembly handlerAssembly)
    {
        var eventTypes = eventAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Event")
                        && !t.IsAbstract
                        && typeof(IEvent).IsAssignableFrom(t));

        Console.WriteLine($"Found {eventTypes.Count()} event(s) to bind.");

        foreach (var eventType in eventTypes)
        {
            var handlerType = FindHandlerForEvent(handlerAssembly, eventType);

            if (handlerType != null)
            {
                var bindMethod = eventListener.GetType()
                    .GetMethod("Bind")
                    ?.MakeGenericMethod(eventType, handlerType);

                bindMethod?.Invoke(eventListener, null);
            }
            else
            {
                Console.WriteLine($"Cannot find handler for {eventType.Name}.");
            }
        }
    }

    private static void RegisterEventHandlers<TReadModel>(
        EventListener<TReadModel> eventListener,
        Assembly eventAssembly,
        Assembly handlerAssembly) where TReadModel : class
    {
        RegisterEventHandlers(eventListener, eventAssembly, handlerAssembly);
    }

    private static Type FindHandlerForEvent(Assembly handlerAssembly, Type eventType)
    {
        var handlerName = string.Concat(eventType.Name.AsSpan(0, eventType.Name.LastIndexOf("Event")), "EventHandler");

        return handlerAssembly.GetTypes()
                              .FirstOrDefault(t => t.Name == handlerName && !t.IsAbstract);
    }
}
