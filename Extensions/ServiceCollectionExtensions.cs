using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Extension methods for setting up CBAR services in an IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the CBAR Client to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">An optional action to configure the <see cref="CbarOptions"/>.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddCbarClient(this IServiceCollection services, Action<CbarOptions>? configureOptions = null)
        {
            // Configure options
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            // Register HttpClient with Options
            services.AddHttpClient<ICbarClient, CbarClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<CbarOptions>>().Value;
                if (!string.IsNullOrEmpty(options.BaseUrl))
                {
                    client.BaseAddress = new Uri(options.BaseUrl);
                }
            });

            return services;
        }
    }
}