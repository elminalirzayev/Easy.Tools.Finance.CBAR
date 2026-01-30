using System.Collections.Generic;
using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR
{
    /// <summary>
    /// Root object representing the XML response from CBAR.
    /// XML Root: &lt;ValCurs Date="dd.MM.yyyy" Name="..." Description="..."&gt;
    /// </summary>
    [XmlRoot("ValCurs")]
    public class CbarResponse
    {
        /// <summary>
        /// Date of the rates (Format: dd.MM.yyyy).
        /// </summary>
        [XmlAttribute("Date")]
        public string? Date { get; set; }

        /// <summary>
        /// Name of the response (e.g., "Azərbaycan Respublikası Mərkəzi Bankının...").
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Description of the response.
        /// </summary>
        [XmlAttribute("Description")]
        public string? Description { get; set; }

        /// <summary>
        /// List of currency types (e.g., Foreign Currencies, Metals).
        /// Initialized to empty list to prevent NullReferenceException.
        /// </summary>
        [XmlElement("ValType")]
        public List<CbarValType> ValTypes { get; set; } = new List<CbarValType>();
    }
}