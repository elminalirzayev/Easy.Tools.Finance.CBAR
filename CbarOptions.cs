namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Configuration options for the CBAR Client.
    /// </summary>
    public class CbarOptions
    {
        /// <summary>
        /// Base URL for the Central Bank of Azerbaijan (CBAR) XML service.
        /// Default: https://www.cbar.az/currencies/
        /// </summary>
        public string BaseUrl { get; set; } = "https://www.cbar.az/currencies/";

        /// <summary>
        /// The number of times to retry the request if it fails due to network issues.
        /// Default: 3
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// The duration to wait (in seconds) between retry attempts.
        /// Default: 1 second
        /// </summary>
        public int RetryDelaySeconds { get; set; } = 1;
    }
}