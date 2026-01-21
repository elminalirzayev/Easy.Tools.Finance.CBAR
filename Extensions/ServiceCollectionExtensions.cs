using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Easy.Tools.Finance.CBAR.Extensions
{
    /// <summary>
    /// Extension methods for setting up CBAR Client in an IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the CBAR Client services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configureOptions">An optional action to configure the <see cref="CbarOptions"/>.</param>
        /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddCbarClient(this IServiceCollection services, Action<CbarOptions>? configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            services.AddHttpClient<ICbarClient, CbarClient>();

            return services;
        }
    }
}