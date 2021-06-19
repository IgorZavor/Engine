using Engine.DAL.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Tests.Services.Companies
{
	public class Tests : CompaniesTestService
	{

		public Tests() : base() {
		
		}

		[Test]
		public async Task GenerateData_CheckCount() {
			var generatedDataCount = 5;
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
			await Generate(generatedDataCount);
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == generatedDataCount);
		}

		[Test]
		public async Task ClearTable_CheckCount()
		{
			// Fill data before cleaning
			var users = new List<Company> {
					new Company() { Id = 1, Country = "USA", Name = "Corporate1", YearFounded = 1980, NumberOfEmployees = 200  },
					new Company() { Id = 2, Country = "Canada", Name = "Corporate2",  YearFounded = 1981, NumberOfEmployees = 300 },
					new Company() { Id = 3, Country = "Canada", Name = "Corporate3", YearFounded = 1980, NumberOfEmployees = 400 },
					new Company() { Id = 4, Country = "Canada", Name = "Corporate4", YearFounded = 1983, NumberOfEmployees = 200},
				};
			Repository.InsertRange(users);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		public async Task FilterBy_CheckSum()
		{
			var columnValuesDictionary = new Dictionary<string, List<string>> {
				{ "Country", new List<string> { "USA" } },
				{ "Name", new List<string> { "Corporate2", } },
				{ "DateFounded", new List<string> { "1980", } },
				{ "NumberOfEmployees", new List<string> { "200" } },
			};
			var expectedResults = new int[] { 600, 300, 600, 400};

			var users = new List<Company> {
					new Company() { Id = 1, Country = "USA", Name = "Corporate1", YearFounded=1980, NumberOfEmployees= 200  },
					new Company() { Id = 2, Country = "Canada", Name = "Corporate2",  YearFounded=1981, NumberOfEmployees= 300 },
					new Company() { Id = 3, Country = "USA", Name = "Corporate3", YearFounded=1980, NumberOfEmployees= 400 },
					new Company() { Id = 4, Country = "Canada", Name = "Corporate4", YearFounded=1983, NumberOfEmployees= 200},
				};
			Repository.InsertRange(users);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			var i = 0;
			foreach (var columnValues in columnValuesDictionary)
			{
				var result = await FilterBy(columnValues.Key, "YearFounded", columnValues.Value);
				Assert.AreEqual(expectedResults[i], result);
				i++;
			}
		}
	}
}
