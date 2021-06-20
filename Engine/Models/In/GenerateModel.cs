using System.Xml.Serialization;

namespace Engine.Models.In
{
	[XmlRoot(ElementName = "Generate")]
	public class GenerateModel
	{
		public string Table { get; set; }

		public int Count { get; set; }
	}
}
