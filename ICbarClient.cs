using Easy.Tools.Finance.CBAR.Models;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Interface for fetching currency rates from CBAR (Central Bank of Azerbaijan).
    /// </summary>
    public interface ICbarClient
    {
        /// <summary>
        /// Retrieves today's exchange rates asynchronously.
        /// Includes built-in retry logic.
        /// </summary>
        /// <returns>A list of currency rates.</returns>
        Task<List<CbarCurrency>> GetTodayRatesAsync();

        /// <summary>
        /// Retrieves exchange rates for a specific date asynchronously.
        /// </summary>
        /// <param name="date">The date for which rates are requested.</param>
        /// <returns>A list of currency rates for the specified date.</returns>
        Task<List<CbarCurrency>> GetRatesByDateAsync(DateTime date);
    }
}