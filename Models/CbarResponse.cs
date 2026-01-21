using System.Xml.Serialization;

namespace Easy.Tools.Finance.CBAR.Models
{
    /// <summary>
    /// Root object representing the XML response from CBAR.
    /// </summary>
    [XmlRoot("ValCurs")]
    public class CbarResponse
    {
        /// <summary>
        /// Date of the rates.
        /// </summary>
        [XmlAttribute("Date")]
        public string? Date { get; set; }

        /// <summary>
        /// List of currency types.
        /// </summary>
        [XmlElement("ValType")]
        public List<CbarValType>? ValTypes { get; set; }
    }
}