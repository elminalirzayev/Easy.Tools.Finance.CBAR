using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Represents a category of currencies (e.g., "Xarici valyutalar", "Bank metalları").
    /// </summary>
    [XmlType("ValType")]
    public class CbarValType
    {
        /// <summary>
        /// Type of the category (e.g., "Xarici valyutalar").
        /// </summary>
        [XmlAttribute("Type")]
        public string? Type { get; set; }

        /// <summary>
        /// List of currencies/metals under this category.
        /// Initialized to empty list to prevent NullReferenceException.
        /// </summary>
        [XmlElement("Valute")]
        public List<CbarCurrency> Currencies { get; set; } = new List<CbarCurrency>();
    }
}