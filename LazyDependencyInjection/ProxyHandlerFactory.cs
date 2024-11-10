using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;

namespace LazyDependencyInjection;

internal class ProxyHandlerFactory
{
    public static ProxyHandler Create(IServiceProvider services, object? serviceKey, Type serviceType)
    {
        var handler = new ProxyHandler((proxy, method, args) =>
            method.Invoke(proxy.GetOrAddState(() => services.GetRequiredKeyedService(proxy.GetDeclaringType(), serviceKey)), args));

        if (!typeof(IDisposable).IsAssignableFrom(serviceType))
            return handler;

        return (proxy, method, args) =>
        {
            if (method.DeclaringType == typeof(IDisposable))
                return null;

            return handler(proxy, method, args);
        };
    }
}
