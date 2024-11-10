# LazyDependencyInjection [![NuGet version](https://badge.fury.io/nu/LazyDependencyInjection.svg?7)](http://badge.fury.io/nu/LazyDependencyInjection)
Lazy injection for Microsoft.Extensions.DependencyInjection.\
Decorates registered services with lazy proxies that instantiate the original service only after the first method or property call.\
Intended to prevent the creation of unused injected dependencies.

### Example
```C#
using LazyDependencyInjection;

var services = new ServiceCollection()
    .AddTransient<IExampleService1, ExampleService1>()
    .AddTransient<IExampleService2, ExampleService2>()
    .AddTransient<IExampleService3, ExampleService3>()
    .AddTransient<ServiceWithManyUnusedDeps>()
     
     // DEFAULT: Adds for any service that has dependencies and injections into other services with multiple methods and dependencies
    .AddLazyProxy()

    // OR: Adds only for specific services
    .AddLazyProxy(typeof(IExampleService1), typeof(IExampleService2))
    
    // OR: Adds with your custom filter
    .AddLazyProxy(x => !x.Descriptor.IsKeyedService && x.Descriptor.HasDependenciesCountGreaterThan(0))

    // OR: Adds for any service that has dependencies and injections into specific services
    .AddLazyProxy(ServiceFilters.HasDependencies()
        .And(ServiceFilters.IsInjectedTo(typeof(ServiceWithManyUnusedDeps))))

    .BuildServiceProvider();
```

[Program.cs](https://github.com/mustaddon/LazyDependencyInjection/blob/main/Example/Program.cs)