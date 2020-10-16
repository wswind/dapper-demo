using Microsoft.Extensions.DependencyInjection;
using System;

namespace DapperDemo.Infra
{
    public class DIManager
    {
        private ServiceProvider _serviceProvider = null;

        public DIManager(Action<ServiceCollection> configureServices)
        {
            var serviceCollection = new ServiceCollection();
            configureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public T For<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }


}
