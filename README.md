# LazyDependencyInjection
Lazy injection for Microsoft.Extensions.DependencyInjection. Decorates registered services with lazy proxies that instantiate the original service only after the first method or property call. Intended to prevent the creation of unused injected dependencies.
