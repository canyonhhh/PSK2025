using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PSK2025.ServiceDefaults.Interceptors;

namespace PSK2025.ServiceDefaults
{
    public static class InterceptorExtensions
    {
        public static IServiceCollection AddBusinessOperationLogging(
            this IServiceCollection services,
            Func<Type, bool>? interfaceFilter = null)
        {
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();

            services.AddScoped<BusinessOperationInterceptor>();

            var serviceDescriptors = services
                .Where(descriptor =>
                    descriptor.ServiceType.IsInterface &&
                    descriptor.ImplementationType != null &&
                    (interfaceFilter == null || interfaceFilter(descriptor.ServiceType)))
                .ToList();

            foreach (var descriptor in serviceDescriptors)
            {
                services.Remove(descriptor);

                services.Add(new ServiceDescriptor(
                    descriptor.ServiceType,
                    serviceProvider =>
                    {
                        var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
                        var actual = serviceProvider.GetRequiredService(descriptor.ImplementationType!);
                        var interceptor = serviceProvider.GetRequiredService<BusinessOperationInterceptor>();

                        return proxyGenerator.CreateInterfaceProxyWithTarget(
                            descriptor.ServiceType,
                            actual,
                            interceptor);
                    },
                    descriptor.Lifetime));

                services.Add(new ServiceDescriptor(
                    descriptor.ImplementationType!,
                    descriptor.ImplementationType!,
                    descriptor.Lifetime));
            }

            return services;
        }

        public static IServiceCollection AddBusinessOperationLogging<TInterface, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();

            services.TryAddScoped<BusinessOperationInterceptor>();

            services.Add(new ServiceDescriptor(
                typeof(TInterface),
                serviceProvider =>
                {
                    var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
                    var actual = serviceProvider.GetRequiredService<TImplementation>();
                    var interceptor = serviceProvider.GetRequiredService<BusinessOperationInterceptor>();

                    return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(actual);
                },
                lifetime));

            return services;
        }
    }
}