using Example;
using LazyDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var services = new ServiceCollection()
    .AddTransient<IExampleService1, ExampleService1>()
    .AddTransient<IExampleService2, ExampleService2>()
    .AddTransient(typeof(IExampleService3<>), typeof(ExampleService3<>))
    .AddTransient<IExampleService3<ContractAB>, ExampleService3<ContractAB>>()
    .AddTransient<IExampleDisposableService, ExampleDisposableService>()
    .AddTransient<ServiceWithManyUnusedDeps>()


    // DEFAULT: Adds for any service that has dependencies
    // and injections into other services with multiple methods and dependencies
    .AddLazyProxy()

    //// OR: Default analog with additional assembly condition for services
    //.AddLazyProxy(Assembly.GetExecutingAssembly())

    //// OR: Adds only for specific services
    //.AddLazyProxy(typeof(IExampleService1), typeof(IExampleService2), typeof(IExampleService3<>))

    //// OR: Adds with your custom filter
    //.AddLazyProxy(x => !x.Descriptor.IsKeyedService && x.Descriptor.HasDependenciesCountGreaterThan(0))

    //// OR: Adds for any service that has dependencies and injections into specific services
    //.AddLazyProxy(ServiceFilters.HasDependencies()
    //    .And(ServiceFilters.IsInjectedTo(typeof(ServiceWithManyUnusedDeps))))

    //// OR: Adds only for specific services that have dependencies
    //.AddLazyProxy(ServiceFilters.HasDependencies()
    //    .And(ServiceFilters.IsServiceType(typeof(IExampleService2), typeof(IExampleService3<>))))

    //// OR: Adds for all
    //.AddLazyProxy(x => true)

    .BuildServiceProvider();



Console.WriteLine("Result: " +
    services.GetRequiredService<ServiceWithManyUnusedDeps>()
        .Method2("some args"));


Console.WriteLine("Result: " +
    services.GetRequiredService<IExampleService1>()
        .Method1(555));


using (var scope = services.CreateScope())
{
    Console.WriteLine("Result: " +
        scope.ServiceProvider.GetRequiredService<IExampleDisposableService>()
            .Method2(777));
}


Console.WriteLine("Result: " +
    services.GetRequiredService<IExampleService3<ContractAB>>()
        .Method3(new()));


