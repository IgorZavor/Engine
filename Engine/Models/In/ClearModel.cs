using System.Xml.Serialization;

namespace Engine.Models.In
{
	[XmlRoot(ElementName = "Clear")]
	public class ClearModel
	{
		public string Table { get; set; }
	}
}
