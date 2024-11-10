
using System.Reflection;

namespace LazyDependencyInjection;

public static class ServiceFilters
{
    public static Func<ServiceInfo, bool> Default()
    {
        return x => x.Descriptor.HasDependenciesCountGreaterThan(0, true)
            && x.Injections.Any(xx => xx.HasMethodsCountGreaterThan(1) && xx.HasDependenciesCountGreaterThan(1, true));
    }

    public static Func<ServiceInfo, bool> HasDependencies(bool excludeServiceProvider = false)
    {
        return x => x.Descriptor.HasDependenciesCountGreaterThan(0, excludeServiceProvider);
    }

    public static Func<ServiceInfo, bool> IsServiceAssembly(params Assembly[] oneOfAssemblies)
    {
        var hashSet = new HashSet<Assembly>(oneOfAssemblies);
        return x => hashSet.Contains(x.Descriptor.ServiceType.Assembly);
    }

    public static Func<ServiceInfo, bool> IsServiceType(params Type[] oneOfTypes)
    {
        var hashSet = new HashSet<Type>(oneOfTypes);
        return x => hashSet.Contains(x.Descriptor.ServiceType);
    }

    public static Func<ServiceInfo, bool> IsInjectedTo(params Type[] oneOfTypes)
    {
        var hashSet = new HashSet<Type>(oneOfTypes);
        return x => x.Injections.Any(xx => hashSet.Contains(xx.ServiceType));
    }

    public static Func<ServiceInfo, bool> Custom(Func<ServiceInfo, bool> predicate)
    {
        return predicate;
    }
}
