using System.ComponentModel.DataAnnotations;

namespace Engine.DAL.Models
{
	public class Log
	{
		public int Id { get; set; }

		[Required]
		public string Author { get; set; }

		[Required]
		public string DateTime { get; set; }

		public string Filter { get; set; }

		[Required]
		public int Sum { get; set; }

	}
}
