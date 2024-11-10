using Microsoft.Extensions.DependencyInjection;

namespace Example;

internal class ExampleService1 : IExampleService1
{
    public object Method1(object a)
    {
        return a;
    }
}

internal class ExampleService2(IExampleService1 exampleService1) : IExampleService2
{
    public object Method2(object a)
    {
        return exampleService1.Method1(a);
    }
}

internal class ExampleService3<T>(
    [FromKeyedServices("test")] IExampleService1 exampleService1,
    IEnumerable<IExampleService2> exampleService2)
    : IExampleService3<T>
{
    public T Method3(T a)
    {
        var m1 = exampleService1.Method1(a!);
        var c = exampleService2.Count();
        return a;
    }
}

internal class ExampleDisposableService(
    IExampleService1 exampleService1,
    IExampleService2 exampleService2)
    : IExampleDisposableService
{
    bool _disposed;


    ~ExampleDisposableService()
    {
        if (!_disposed)
            Dispose();
    }

    public void Dispose()
    {
        if (_disposed) throw new Exception("Already disposed!");
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public object Method1(object a)
    {
        return exampleService1.Method1(a);
    }

    public object Method2(object a)
    {
        return exampleService2.Method2(a);
    }
}


internal class ServiceWithManyUnusedDeps(
    IExampleService1 exampleService1,
    IExampleService2 exampleService2,
    IExampleService3<ContractA> exampleService3A,
    IExampleService3<ContractB> exampleService3B,
    IExampleDisposableService exampleDisposable)
{
    public object Method1(object a)
    {
        return exampleService1.Method1(a) ?? exampleDisposable.Method1(a);
    }

    public object Method2(object a)
    {
        return exampleService2.Method2(a);
    }

    public ContractA Method3A(ContractA a)
    {
        return exampleService3A.Method3(a);
    }

    public ContractB Method3B(ContractB a)
    {
        return exampleService3B.Method3(a);
    }

}
