using LazyDependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class LazyDependencyInjectionServiceCollectionExtensions
{
    /// <summary>
    /// Adds LazyProxy for any service that has dependencies and injections into other services with multiple methods and dependencies
    /// </summary>
    public static IServiceCollection AddLazyProxy(this IServiceCollection services)
    {
        return AddLazyProxy(services, ServiceFilters.Default());
    }

    /// <summary>
    /// Like AddLazyProxy() but with additional assembly condition for services
    /// </summary>
    public static IServiceCollection AddLazyProxy(this IServiceCollection services, params Assembly[] serviceAssemblies)
    {
        return AddLazyProxy(services, ServiceFilters.IsServiceAssembly(serviceAssemblies).And(ServiceFilters.Default()));
    }

    /// <summary>
    /// Adds LazyProxy only for specified services
    /// </summary>
    public static IServiceCollection AddLazyProxy(this IServiceCollection services, params Type[] serviceTypes)
    {
        return AddLazyProxy(services, ServiceFilters.IsServiceType(serviceTypes));
    }

    /// <summary>
    /// Adds LazyProxy for selected services only
    /// </summary>
    public static IServiceCollection AddLazyProxy(this IServiceCollection services, Func<ServiceInfo, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var injectionsMap = services.CreateInjectionsMap();

        ServiceRegistrar.Register(services, x => filter(new ServiceInfo(x, () => injectionsMap.Value(x))));

        return services;
    }
}