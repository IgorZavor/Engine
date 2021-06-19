using System.Collections.Generic;

namespace Engine.Models.In
{
	public class FilterByIn
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
