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
}
