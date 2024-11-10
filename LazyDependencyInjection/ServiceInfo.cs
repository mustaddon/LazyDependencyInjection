using Microsoft.Extensions.DependencyInjection;

namespace LazyDependencyInjection;


public sealed class ServiceInfo
{
    internal ServiceInfo(ServiceDescriptor descriptor, Func<IEnumerable<ServiceDescriptor>> injectionsGetter)
    {
        _descriptor = descriptor;
        _injectionsGetter = injectionsGetter;
    }

    private readonly ServiceDescriptor _descriptor;
    private readonly Func<IEnumerable<ServiceDescriptor>> _injectionsGetter;

    public ServiceDescriptor Descriptor => _descriptor;
    public IEnumerable<ServiceDescriptor> Injections => _injectionsGetter();
}