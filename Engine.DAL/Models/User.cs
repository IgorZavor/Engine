using System.ComponentModel.DataAnnotations;

namespace Engine.DAL.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required]
		public int Age { get; set; }

		[Required]
		public string Name { get; set; }
		public string Surname { get; set; }

		public string Country { get; set; }
	}
}
