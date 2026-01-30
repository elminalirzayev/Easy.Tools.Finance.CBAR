using System.Globalization;
using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Represents a single currency or metal unit from CBAR XML data.
    /// </summary>
    [XmlType("Valute")]
    public class CbarCurrency
    {
        /// <summary>
        /// Currency or Metal Code (e.g., USD, XAU, XAG).
        /// </summary>
        [XmlAttribute("Code")]
        public string? Code { get; set; }

        /// <summary>
        /// Raw nominal amount string from XML (e.g. "1", "100", "1 t.u.").
        /// </summary>
        [XmlElement("Nominal")]
        public string? NominalStr { get; set; }

        /// <summary>
        /// Name of the currency/metal (e.g., "ABŞ dolları", "Qızıl").
        /// </summary>
        [XmlElement("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Raw exchange rate value string from XML (e.g., "1.7000").
        /// </summary>
        [XmlElement("Value")]
        public string? ValueStr { get; set; }

        /// <summary>
        /// Type of the currency (e.g., "Xarici valyutalar").
        /// This field is populated manually by the Client logic, not by XML deserialization.
        /// </summary>
        [XmlIgnore]
        public string? CurrencyType { get; set; }

        // --- Helper Properties ---

        /// <summary>
        /// The exchange rate parsed as decimal. Returns 0 if parsing fails.
        /// </summary>
        [XmlIgnore]
        public decimal Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ValueStr)) return 0m;

                // Fast path: InvariantCulture is used because CBAR always sends dot (.) separator.
                return decimal.TryParse(ValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var val)
                    ? val
                    : 0m;
            }
        }

        /// <summary>
        /// The nominal amount parsed as integer. 
        /// Handles suffixes like "1 t.u." efficiently without string allocations.
        /// </summary>
        [XmlIgnore]
        public int Nominal => ParseNominal(NominalStr);

        /// <summary>
        /// Parses the nominal string efficiently.
        /// </summary>
        private static int ParseNominal(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return 1;

#if NET6_0_OR_GREATER || NETSTANDARD2_1
            // Modern .NET: Zero-Allocation using Span<char>
            ReadOnlySpan<char> span = val.AsSpan();

            int spaceIndex = span.IndexOf(' ');
            ReadOnlySpan<char> numberPart = spaceIndex > 0 ? span.Slice(0, spaceIndex) : span;

            return int.TryParse(numberPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : 1;
#else
            // Legacy .NET: Efficient substring
            int spaceIndex = val!.IndexOf(' ');
            string numberPart = spaceIndex > 0 ? val.Substring(0, spaceIndex) : val;

            return int.TryParse(numberPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) 
                ? result 
                : 1;
#endif
        }

    }
}