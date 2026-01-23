using Easy.Tools.Finance.CBAR.Models;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;

#if NETFRAMEWORK
using System.Net.Http;
#endif

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Implementation of the CBAR Client to fetch currency data via HTTP.
    /// </summary>
    public class CbarClient : ICbarClient
    {
        private readonly HttpClient _httpClient;
        private readonly CbarOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CbarClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client instance.</param>
        /// <param name="options">Configuration options.</param>
        public CbarClient(HttpClient httpClient, IOptions<CbarOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task<List<CbarCurrency>> GetTodayRatesAsync()
        {
            return await GetRatesByDateAsync(DateTime.Now);
        }


        /// <inheritdoc />
        public async Task<List<CbarCurrency>> GetRatesByDateAsync(DateTime date)
        {
            string dateStr = date.ToString("dd.MM.yyyy");
            string url = $"{_options.BaseUrl.TrimEnd('/')}/{dateStr}.xml";

            for (int i = 0; i < _options.RetryCount; i++)
            {
                try
                {
                    using var responseStream = await _httpClient.GetStreamAsync(url);
                    XmlSerializer serializer = new XmlSerializer(typeof(CbarResponse));

                    if (serializer.Deserialize(responseStream) is CbarResponse response && response.ValTypes != null)
                    {
                        // While flattening, we assign the Parent Type (ValType) to the Child (Currency).
                        var allCurrencies = new List<CbarCurrency>();

                        foreach (var valType in response.ValTypes)
                        {
                            if (valType.Currencies == null) continue;

                            foreach (var currency in valType.Currencies)
                            {
                                // Assign the category (e.g., "Bank metalları")
                                currency.CurrencyType = valType.Type;
                                allCurrencies.Add(currency);
                            }
                        }

                        return allCurrencies;
                    }
                }
                catch (HttpRequestException)
                {
                    if (i == _options.RetryCount - 1) throw;
                    await Task.Delay(TimeSpan.FromSeconds(_options.RetryDelaySeconds));
                }
                catch (Exception) { throw; }
            }

            return new List<CbarCurrency>();
        }
    }
}