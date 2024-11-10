namespace Example;

internal interface IExampleService1
{
    object Method1(object a);
}

internal interface IExampleService2
{
    object Method2(object a);
}

internal interface IExampleService3<T>
{
    T Method3(T a);
}


internal interface IExampleDisposableService : IExampleService1, IExampleService2, IDisposable
{

}