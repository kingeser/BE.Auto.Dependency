using System.Reflection;
using Be.Auto.Dependency.Attributes;
using Be.Auto.Dependency.Interfaces;

namespace Be.Auto.Dependency.Extensions;


public static class AutoDependencyBuilderExtensions
{
    public static IServiceCollection AddAutoDependency(this IServiceCollection serviceCollection)
    {
        var types =
            from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
            where typeof(IAutoDependency).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract &&
                  !t.IsGenericType && t.IsPublic && t.IsClass
            let interfaces = t.GetInterfaces().Where(x => !$"{x.FullName}".StartsWith("Be.Auto"))
            select new
            {
                Type = t,
                ServiceType = interfaces.FirstOrDefault() ?? t,
                Scope = _FindScope(t)
            };

        foreach (var type in types)
        {
            var serviceDescriptor = ServiceDescriptor.Describe(type.ServiceType, type.Type, type.Scope);
            serviceCollection.Add(serviceDescriptor);
        }

        return serviceCollection;
    }

    private static ServiceLifetime _FindScope(Type type)
    {
        if (typeof(ISingletonDependency).IsAssignableFrom(type) || type.GetCustomAttribute<SingletonDependencyAttribute>() != null)
        {
            return ServiceLifetime.Singleton;
        }
        if (typeof(ITransientDependency).IsAssignableFrom(type) || type.GetCustomAttribute<TransientDependencyAttribute>() != null)
        {
            return ServiceLifetime.Transient;
        }
        return typeof(IScopedDependency).IsAssignableFrom(type) || type.GetCustomAttribute<ScopedDependencyAttribute>() != null ? ServiceLifetime.Scoped : ServiceLifetime.Transient;
    }
}
