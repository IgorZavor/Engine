using System.Collections.Generic;
using System.Xml.Serialization;

namespace Engine.Models.In
{
	[XmlRoot(ElementName = "FilterBy")]
	public class FilterByModel
	{
		public string Author { get; set; }

		public List<Filter> Filters { get; set; }

		public string Table { get; set; }

		public string FilterColumn { get; set; }

		public string SummaryColumn { get; set; }
	}

	public class Filter 
	{
		public string Value { get; set; }
	}
}
