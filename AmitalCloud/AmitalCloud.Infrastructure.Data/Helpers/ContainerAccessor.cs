using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class ContainerAccessor : IContainerAccessor
    {

        #region Members
        private static IServiceProvider _serviceProvider;
        private static IServiceCollection _services;
        #endregion

        #region Properties

        /// <summary>
        /// The Unity container for the current application
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    InitContainer();
                }
                return _serviceProvider;
            }
        }
        #endregion

        #region IContainerAccessor Members

        /// <summary>
        /// Returns the Unity container of the application 
        /// </summary>
        /// 
        /*
        IServiceProvider IContainerAccessor.ServiceProvider
        {
            get { return ServiceProvider; }
        }

        IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }*/

        #endregion



        public static void InitContainer()
        {
            if (_serviceProvider == null)
            {
                // Initialize ServiceCollection
                _services = new ServiceCollection();
                _services.AddScoped<IBlobService>(provider =>
                {
                    var storageServiceMode = AmitalCloudSettings.StorageServiceMode;
                    switch (storageServiceMode)
                    {
                        case "azure":
                            return new AzureBlobService();
                        case "db":
                            return new DatabaseBlobService();
                        case "fs":
                            return new FileSystemBlobService();
                        default:
                            return new AzureBlobService();
                    }
                });

                _services.AddScoped<IQueueService>(provider =>
                {
                    var queueServiceMode = AmitalCloudSettings.QueueServiceMode;
                    switch (queueServiceMode)
                    {
                        case "azure":
                            return new AzureQueueService();
                        case "db":
                            return new DbQueueService();
                        default:
                            return new AzureQueueService();
                    }
                });

                // Build the service provider
                _serviceProvider = _services.BuildServiceProvider();
            }

        }

        public static void RegisterTypeFactory<TFrom, TTo>(string key, TTo _factoryObject)
            where TFrom : class
            where TTo : class, TFrom
        {
            _services.AddScoped<TFrom, TTo>();
            _serviceProvider = _services.BuildServiceProvider(); // Rebuild the provider after registration
        }

        public static void CleanUp()
        {
            if (_serviceProvider != null)
            {
                (_serviceProvider as IDisposable)?.Dispose();
            }
        }

    }

    public static class Extension
    {
        public static string NameOf(this object o)
        {
            var o1 = o.GetType().UnderlyingSystemType.Name;
            return o.GetType().Name;
        }
        public static I ResolveSafe<I>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService<I>() != null)
            {
                return serviceProvider.GetService<I>();
            }
            return default(I);
        }
    }
}
