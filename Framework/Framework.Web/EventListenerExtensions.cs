using Framework.Core;
using Framework.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Framework.Web
{
    public static class EventListenerExtensions
    {
        public static IServiceCollection AddEventListener<TReadModel>(
            this IServiceCollection services,
            Assembly assembly) where TReadModel : class
        {
            services.AddSingleton<IEventListener, EventListener<TReadModel>>(provider =>
            {
                var readRepository = provider.GetRequiredService<IReadRepository<TReadModel>>();
                var eventListener = new EventListener<TReadModel>(readRepository);

                // Register events and handlers in the same assembly
                RegisterEventHandlers(eventListener, assembly, assembly);

                return eventListener;
            });

            return services;
        }

        public static IServiceCollection AddEventListener<TReadModel>(
            this IServiceCollection services,
            Assembly eventAssembly,
            Assembly handlerAssembly) where TReadModel : class
        {
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

        private static void RegisterEventHandlers<TReadModel>(
            EventListener<TReadModel> eventListener,
            Assembly eventAssembly,
            Assembly handlerAssembly) where TReadModel : class
        {
            // Find all event types in the event assembly that match the naming convention
            var eventTypes = eventAssembly.GetTypes()
                .Where(t => t.Name.EndsWith("Event")
                            && !t.IsAbstract
                            && typeof(IEvent).IsAssignableFrom(t));

            foreach (var eventType in eventTypes)
            {
                // Find the corresponding handler in the handler assembly based on naming convention
                var handlerName = eventType.Name.Replace("Event", "EventHandler");
                var handlerType = handlerAssembly.GetTypes()
                    .FirstOrDefault(t => t.Name == handlerName && !t.IsAbstract);

                if (handlerType != null)
                {
                    // Use reflection to invoke the Bind method
                    var bindMethod = typeof(EventListener<TReadModel>)
                        .GetMethod("Bind")
                        ?.MakeGenericMethod(eventType, handlerType);

                    bindMethod?.Invoke(eventListener, null);
                }
            }
        }
    }
}
