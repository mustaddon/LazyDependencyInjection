using LazyDependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class LazyDependencyInjectionServiceCollectionExtensions
{
    public static IServiceCollection AddLazyProxy(this IServiceCollection services, params Type[] serviceTypes)
    {
        return AddLazyProxy(services, serviceTypes.Length == 0 ? null : ServiceFilters.IsServiceType(serviceTypes));
    }

    public static IServiceCollection AddLazyProxy(this IServiceCollection services, Func<ServiceInfo, bool>? filter)
    {
        filter ??= ServiceFilters.Default();

        var injectionsMap = services.CreateInjectionsMap();

        ServiceRegistrar.Register(services, x => filter(new ServiceInfo(x, () => injectionsMap.Value(x))));

        return services;
    }
}