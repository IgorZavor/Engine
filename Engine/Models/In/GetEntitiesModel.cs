using System.Xml.Serialization;

namespace Engine.Models.In
{
	[XmlRoot(ElementName = "Entities")]
	public class GetEntitiesModel
	{
		public string Table { get; set; }
	}
}
