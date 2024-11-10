namespace LazyDependencyInjection;

public static class ServiceFilterExtensions
{
    public static Func<ServiceInfo, bool> And(this Func<ServiceInfo, bool> a, Func<ServiceInfo, bool> b)
    {
        return x => a(x) && b(x);
    }

    public static Func<ServiceInfo, bool> Or(this Func<ServiceInfo, bool> a, Func<ServiceInfo, bool> b)
    {
        return x => a(x) || b(x);
    }
}
