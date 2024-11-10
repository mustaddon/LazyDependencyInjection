using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;

namespace LazyDependencyInjection;

internal static class ServiceRegistrar
{
    public static void Register(IServiceCollection services, Func<ServiceDescriptor, bool> filter)
    {
        var count = services.Count;

        for (var i = 0; i < count; i++)
        {
            var descriptor = services[i];

            if (!descriptor.ServiceType.IsInterface || descriptor.IsProxyKeyedService())
                continue;

            var implementationType = descriptor.GetImplementationType();

            if (implementationType is null && !descriptor.HasImplementationFactory())
                continue;

            if (implementationType?.IsLazyDI() == true || !filter(descriptor))
                continue;

            var serviceKey = ReplaceToLazyProxy(services, descriptor, i);

            if (implementationType is not null)
            {
                services.AddKeyedTransient(descriptor.ServiceType, serviceKey, implementationType);
            }
            else
            {
                services.AddKeyedTransient(descriptor.ServiceType, serviceKey, descriptor.IsKeyedService
                    ? descriptor.KeyedImplementationFactory!
                    : (s, k) => descriptor.ImplementationFactory!(s));
            }
        }
    }

    static string ReplaceToLazyProxy(IServiceCollection services, ServiceDescriptor descriptor, int i)
    {
        var serviceKey = string.Concat(KEY_PREFIX, Guid.NewGuid().ToString("N"));

        var lazyProxyType = ProxyFactory.CreateType(descriptor.ServiceType,
            new CustomAttributeBuilder(typeof(FromKeyedServicesAttribute).GetConstructor([typeof(object)])!, [serviceKey]));

        services.RemoveAt(i);
        services.Insert(i, new ServiceDescriptor(
            descriptor.ServiceType,
            descriptor.ServiceKey,
            lazyProxyType,
            descriptor.Lifetime));

        services.AddKeyedTransient(serviceKey, (s, k) => ProxyHandlerFactory.Create(s, k, descriptor.ServiceType));

        return serviceKey;
    }

    internal const string KEY_PREFIX = "_proxy_lazy_";
}
