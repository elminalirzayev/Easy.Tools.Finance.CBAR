using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR.Models
{
    /// <summary>
    /// Represents a category of currencies (e.g., Foreign Currencies, Bank Metals).
    /// </summary>
    [XmlType("ValType")]
    public class CbarValType
    {
        /// <summary>
        /// Type of the category.
        /// </summary>
        [XmlAttribute("Type")]
        public string? Type { get; set; }

        /// <summary>
        /// List of currencies under this category.
        /// </summary>
        [XmlElement("Valute")]
        public List<CbarCurrency>? Currencies { get; set; }
    }
}