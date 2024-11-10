using LazyDependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class LazyDependencyInjectionServiceCollectionExtensions
{
    public static IServiceCollection AddLazyProxy(this IServiceCollection services, Func<ServiceInfo, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var injectionsMap = services.CreateInjectionsMap();

        ServiceRegistrar.Register(services, x => filter(new ServiceInfo(x, () => injectionsMap.Value(x))));

        return services;
    }

    public static IServiceCollection AddLazyProxy(this IServiceCollection services)
    {
        return AddLazyProxy(services, ServiceFilters.Default());
    }

    public static IServiceCollection AddLazyProxy(this IServiceCollection services, params Assembly[] serviceAssemblies)
    {
        return AddLazyProxy(services, ServiceFilters.IsServiceAssembly(serviceAssemblies).And(ServiceFilters.Default()));
    }

    public static IServiceCollection AddLazyProxy(this IServiceCollection services, params Type[] serviceTypes)
    {
        return AddLazyProxy(services, ServiceFilters.IsServiceType(serviceTypes));
    }
}