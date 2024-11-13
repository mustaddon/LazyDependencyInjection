using DispatchProxyAdvanced;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LazyDependencyInjection;

public static class TypeExtensions
{
    public static bool IsLazyDI(this Type type)
    {
        return (type.GetProxyHandlerParameter()
            ?.GetCustomAttribute<FromKeyedServicesAttribute>()
            ?.Key as string)
            ?.StartsWith(ServiceRegistrar.KEY_PREFIX) == true;
    }

    public static bool IsSystemAssembly(this Type type)
    {
        var assemblyName = type.Assembly.FullName;

        if (assemblyName == null)
            return false;

        return assemblyName.StartsWith("System.") || assemblyName.StartsWith("Microsoft.");
    }
}
