using Framework.Core;
using System.Reflection;

namespace SampleApp.Web;

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
}
