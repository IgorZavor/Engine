using System.ComponentModel.DataAnnotations;

namespace Engine.DAL.Models
{
	public class Company
	{
		public int Id { get; set; }

		[Required]
		public int NumberOfEmployees { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Country { get; set; }

		[Required]
		public int YearFounded { get; set; }

	}
}
