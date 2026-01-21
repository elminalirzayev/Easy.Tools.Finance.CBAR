using System.Globalization;
using System.Text.RegularExpressions; // Regex eklendi
using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR.Models
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
        /// Nominal amount string from XML (e.g. "1", "100", "1 t.u.").
        /// </summary>
        [XmlElement("Nominal")]
        public string? NominalStr { get; set; }

        /// <summary>
        /// Name of the currency/metal (e.g., "ABŞ dolları", "Qızıl").
        /// </summary>
        [XmlElement("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Exchange rate value string from XML.
        /// </summary>
        [XmlElement("Value")]
        public string? ValueStr { get; set; }

        // --- New Property: Type ---

        /// <summary>
        /// Type of the currency (e.g., "Xarici valyutalar", "Bank metalları").
        /// Filled automatically during parsing.
        /// </summary>
        [XmlIgnore]
        public string? CurrencyType { get; set; }

        // --- Helper Properties ---

        /// <summary>
        /// The exchange rate parsed as decimal.
        /// </summary>
        [XmlIgnore]
        public decimal Value => ParseDecimal(ValueStr);

        /// <summary>
        /// The nominal amount parsed as integer. 
        /// Handles "1 t.u." by taking the first numeric part.
        /// </summary>
        [XmlIgnore]
        public int Nominal => ParseNominal(NominalStr);

        /// <summary>
        /// Parses string to decimal using InvariantCulture.
        /// </summary>
        private decimal ParseDecimal(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return 0;
            return decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        /// <summary>
        /// Parses nominal string, handling suffixes like " t.u.".
        /// </summary>
        private int ParseNominal(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return 1;

            // Simple Logic: Take the first part before space (e.g., "1 t.u." -> "1")
            var firstPart = val?.Split(' ')[0];

            return int.TryParse(firstPart, out int result) ? result : 1;
        }
    }
}