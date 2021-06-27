using Engine.DAL.Models;
using Engine.DAL.Repositories.Users;
using Engine.Providers;
using Microsoft.Extensions.Logging;
using System;

namespace Engine.Services.WorkingServices.Users
{
	public class UsersService : WorkingBaseService
	{
		private Random _random;
		public UsersService(IUsersRepository repository, ILogger<UsersService> logger) : base(repository, logger)
		{
			_random = new Random();
		}

		protected override object CreateEntity()
		{
			var age = GetAge();
			var userName = GetName();
			var userSurname = GetSurname();
			var country = GetCountry();
			return new User() { Age = age, Name = userName, Surname = userSurname, Country = country };
		}

		private string GetName() {
			var names = ResourcesProvider.Instance.Names;
			return names[_random.Next(0, names.Length)];
		}

		private string GetSurname() {
			var surnames = ResourcesProvider.Instance.Surnames;
			return surnames[_random.Next(0, surnames.Length)];
		}

		private int GetAge()
		{
			return _random.Next(15, 100);
		}

		private string GetCountry() {
			var countries = ResourcesProvider.Instance.Countries;
			return countries[_random.Next(0, countries.Length)];
		}

		protected override int GetValue(object entity, string column)
		{
			var summartColumn = (Columns)Enum.Parse(typeof(Columns), column, true);
			switch (summartColumn) {
				case Columns.Age:
					return ((User)entity).Age;
				case Columns.Id:
					return ((User)entity).Id;
				default:
					throw new InvalidOperationException($"{TableName} table doesn't contain the {column} column or summation is not supported for {column} column ");
			}
			
		}

		protected override string TableName => "Users";
	}
}
