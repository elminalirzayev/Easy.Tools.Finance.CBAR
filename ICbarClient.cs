#if NETFRAMEWORK
using System.Net.Http;
#endif

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Interface for fetching currency rates from CBAR (Central Bank of Azerbaijan).
    /// </summary>
    public interface ICbarClient
    {
        /// <summary>
        /// Retrieves today's exchange rates asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A list of currency rates.</returns>
        /// <exception cref="HttpRequestException">Thrown when the connection to CBAR fails after retries.</exception>
        Task<List<CbarCurrency>> GetTodayRatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves exchange rates for a specific date asynchronously.
        /// </summary>
        /// <param name="date">The date for which rates are requested.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A list of currency rates for the specified date.</returns>
        /// <exception cref="HttpRequestException">Thrown when the connection to CBAR fails after retries.</exception>
        Task<List<CbarCurrency>> GetRatesByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}