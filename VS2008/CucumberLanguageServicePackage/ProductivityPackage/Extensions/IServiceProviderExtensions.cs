using System;

namespace ProductivityPackage
{
    /// <summary>
    /// Extension methods for IServiceProvider
    /// </summary>
	public static class IServiceProviderExtensions
	{
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
		public static TService GetService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="SInterface">The type of the interface.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
		public static TInterface GetService<SInterface, TInterface>(this IServiceProvider serviceProvider)
		{
			return (TInterface)serviceProvider.GetService(typeof(SInterface));
		}

        /// <summary>
        /// Tries to the get service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
		public static TService TryGetService<TService>(this IServiceProvider serviceProvider)
		{
			object service = serviceProvider.GetService(typeof(TService));

			if(service == null)
			{
				return default(TService);
			}

			return (TService)service;
		}

        /// <summary>
        /// Tries to the get service.
        /// </summary>
        /// <typeparam name="SInterface">The type of the interface.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
		public static TInterface TryGetService<SInterface, TInterface>(this IServiceProvider serviceProvider)
		{
			object service = serviceProvider.GetService(typeof(SInterface));

			if(service == null)
			{
				return default(TInterface);
			}

			return (TInterface)service;
		}
	}
}