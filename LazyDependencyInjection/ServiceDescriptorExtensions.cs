using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LazyDependencyInjection;

public static class ServiceDescriptorExtensions
{
    public static ParameterInfo[] GetConstructorParameters(this ServiceDescriptor descriptor)
    {
        return descriptor
            .GetImplementationType()?
            .GetConstructors()
            .FirstOrDefault()?
            .GetParameters() ?? [];
    }

    public static bool IsLazyDI(this ServiceDescriptor descriptor)
    {
        return descriptor.GetImplementationType()?.IsLazyDI() == true;
    }

    internal static bool IsProxyKeyedService(this ServiceDescriptor descriptor)
    {
        return descriptor.IsKeyedService
            && (descriptor.ServiceKey as string)?.StartsWith("_proxy_") == true;
    }

    internal static Type? GetImplementationType(this ServiceDescriptor descriptor)
    {
        return descriptor.IsKeyedService
            ? descriptor.KeyedImplementationType
            : descriptor.ImplementationType;
    }

    internal static bool HasImplementationFactory(this ServiceDescriptor descriptor)
    {
        return descriptor.IsKeyedService
            ? descriptor.KeyedImplementationFactory != null
            : descriptor.ImplementationFactory != null;
    }

    public static bool HasDependenciesCountGreaterThan(this ServiceDescriptor descriptor, int limit, bool excludeServiceProvider = false)
    {
        if (excludeServiceProvider)
            return HasDependenciesCountGreaterThan(descriptor, limit, static p => p.ParameterType != typeof(IServiceProvider));

        return HasDependenciesCountGreaterThan(descriptor, limit, null);
    }

    public static bool HasDependenciesCountGreaterThan(this ServiceDescriptor descriptor, int limit, Func<ParameterInfo, bool>? where)
    {
        var count = 0;

        foreach (var p in descriptor.GetConstructorParameters().Where(where ?? (static p => true)))
        {
            if (p.ParameterType.IsInterface
                && p.ParameterType.IsGenericType
                && p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            count++;
        }

        return count > limit;
    }

    public static bool HasMethodsCountGreaterThan(this ServiceDescriptor descriptor, int limit)
    {
        var count = 0;

        foreach (var p in descriptor.GetMethods())
        {
            if (++count > limit)
                return true;
        }

        return false;
    }

    public static IEnumerable<MethodInfo> GetMethods(this ServiceDescriptor descriptor)
    {
        var implementationType = descriptor.GetImplementationType();

        if (implementationType == null)
        {
            if (descriptor.ServiceType.IsInterface)
            {
                return descriptor.ServiceType
                    .GetMethods()
                    .Concat(descriptor.ServiceType
                        .GetInterfaces()
                        .Where(i => i != typeof(IDisposable) && i != typeof(IAsyncDisposable))
                        .SelectMany(x => x.GetMethods()))
                    .Where(m => !m.IsStatic && (m.IsPublic || m.IsFamily));
            }

            implementationType = descriptor.ServiceType;
        }

        return implementationType
            .GetMethods()
            .Where(m => !m.IsStatic && (m.IsPublic || m.IsFamily) && !_ignoredMethodNames.Value.Contains(m.Name));
    }

    static readonly Lazy<HashSet<string>> _ignoredMethodNames = new(() => new(
        new[] {
            typeof(object),
            typeof(IDisposable),
            typeof(IAsyncDisposable)
        }
        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        .Select(x => x.Name)));

}
