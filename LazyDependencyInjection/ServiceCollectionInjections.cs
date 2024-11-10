using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LazyDependencyInjection;

internal static class ServiceCollectionInjections
{
    internal static Lazy<Func<ServiceDescriptor, IEnumerable<ServiceDescriptor>>> CreateInjectionsMap(this IServiceCollection services)
    {
        return new(() =>
        {
            var injectionsMap = services
                .Where(x => !x.IsLazyDI())
                .SelectMany(x => x
                    .GetConstructorParameters()
                    .Where(p => p.ParameterType.IsInterface && p.ParameterType != typeof(IServiceProvider))
                    .Select(p => new
                    {
                        Descriptor = x,
                        ServiceType = p.ParameterType,
                        ServiceKey = p.GetCustomAttribute<FromKeyedServicesAttribute>()?.Key,
                    }))
                .SelectMany(x => x
                    .ServiceType
                    .GetInjectableVariants()
                    .Select(t => new
                    {
                        x.Descriptor,
                        ServiceType = t,
                        x.ServiceKey,
                    }))
                .SelectMany(x => x
                    .ServiceKey
                    .GetKeyVariants()
                    .Select(k => new
                    {
                        x.Descriptor,
                        x.ServiceType,
                        ServiceKey = k,
                    }))
                .GroupBy(x => new { x.ServiceType, x.ServiceKey })
                .ToDictionary(g => g.Key, g => g.Select(x => x.Descriptor));

            return x =>
            {
                if (injectionsMap.TryGetValue(
                    new { x.ServiceType, x.ServiceKey },
                    out var injections))
                    return injections;

                if (x.ServiceType.IsGenericType && injectionsMap.TryGetValue(
                    new { ServiceType = x.ServiceType.GetGenericTypeDefinition(), x.ServiceKey },
                    out injections))
                    return injections;

                return [];
            };
        });
    }

    static IEnumerable<Type> GetInjectableVariants(this Type type)
    {
        if (!type.IsInterface)
            yield break;

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(IEnumerable<>))
            {
                var enumerableType = type.GetGenericArguments()[0];

                if (enumerableType.IsInterface)
                    yield return enumerableType;

                yield break;
            }

            yield return genericTypeDefinition;
        }

        yield return type;
    }

    static IEnumerable<object?> GetKeyVariants(this object? key)
    {
        if (key != null)
            yield return null;

        yield return key;
    }
}
