using Engine.DAL.Models;
using Engine.DAL.Repositories.Companies;
using Engine.Providers;
using Microsoft.Extensions.Logging;
using System;

namespace Engine.Services.Companies
{
	public class CompaniesService : ILogsService
	{
		private Random _random;
		public CompaniesService(ICompaniesRepository repository, ILogger<CompaniesService> logger) : base(repository, logger)
		{
			_random = new Random();
		}

		protected override object CreateEntity()
		{
			var name = GetName();
			var country = GetCountry();
			var dateFounded = GetDateFounded();
			var numberOfEmployees = GetNumberOfEmployees();
			return new Company() { Country = country, Name = name, YearFounded = dateFounded, NumberOfEmployees = numberOfEmployees };
		}

		private string GetName()
		{
			var name = "Company" + _random.Next(0, 10000);
			return name;
		}

		private string GetCountry()
		{
			var countries = ResourcesProvider.Instance.Countries;
			return countries[_random.Next(0, countries.Length)];
		}

		private int GetDateFounded()
		{
			return _random.Next(1940, 2021);
		}

		private int GetNumberOfEmployees()
		{
			return _random.Next(10, 200);
		}

		protected override int GetValue(object entity, string column)
		{
			var summartColumn = (SummaryColumns)Enum.Parse(typeof(SummaryColumns), column, true);
			switch (summartColumn)
			{
				case SummaryColumns.Id:
					return ((Company)entity).Id;
				case SummaryColumns.NumberOfEmployees:
					return ((Company)entity).NumberOfEmployees;
				case SummaryColumns.YearFounded:
					return ((Company)entity).YearFounded;
				default:
					throw new InvalidOperationException($"Users table doens't contain the {column} colunm or summation is not supported by {column} column ");
			}
		}

		protected override string TableName => "Companies";
	}
}
