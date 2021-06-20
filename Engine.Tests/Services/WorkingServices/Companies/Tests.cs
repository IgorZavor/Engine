using Engine.DAL.Models;
using Engine.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices.Companies
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
			var data = new List<Company> {
					new Company() { Id = 1, Country = "USA", Name = "Corporate1", YearFounded = 1980, NumberOfEmployees = 200  },
					new Company() { Id = 2, Country = "Canada", Name = "Corporate2",  YearFounded = 1981, NumberOfEmployees = 300 },
					new Company() { Id = 3, Country = "Canada", Name = "Corporate3", YearFounded = 1980, NumberOfEmployees = 400 },
					new Company() { Id = 4, Country = "Canada", Name = "Corporate4", YearFounded = 1983, NumberOfEmployees = 200},
				};
			Repository.InsertRange(data);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		[TestCase("Country", "id", "USA")]
		[TestCase("Name", "NumberOfEmployees", "Corporate2", "Corporate3")]
		[TestCase("YearFounded", "id", "100")]
		[TestCase("Id", "YearFounded", "1980")]
		[TestCase("NumberOfEmployees", "YearFounded", "200")]
		public async Task FilterBy_CheckSum(string filterColumn, string sumColumn, string value, string? value1)
		{
			var data = new List<Company> {
					new Company() { Id = 1, Country = "USA", Name = "Corporate1", YearFounded=1980, NumberOfEmployees= 200  },
					new Company() { Id = 2, Country = "Canada", Name = "Corporate2",  YearFounded=1981, NumberOfEmployees= 300 },
					new Company() { Id = 3, Country = "USA", Name = "Corporate3", YearFounded=1980, NumberOfEmployees= 400 },
					new Company() { Id = 4, Country = "Canada", Name = "Corporate4", YearFounded=1983, NumberOfEmployees= 200},
				};
			Repository.InsertRange(data);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			var filtered = FilterHelper.GetFilteredCompanies(data, filterColumn, new List<string> { value1, value });
			var expected = SumHelper.GetCompaniesSum(filtered, sumColumn);
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value });
			Assert.AreEqual(expected, actual);
		}
	}
}
