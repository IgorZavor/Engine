using Engine.DAL.Models;
using Engine.Tests.Repositories.Companies;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices.Companies
{
	public class Tests : CompaniesTestService
	{

		public Tests() : base() {
		
		}

		[SetUp]
		public void SetUp() 
		{
			_data = new List<Company> {
					new Company() { Id = 1, Country = "USA", Name = "Corporate1", YearFounded=1980, NumberOfEmployees= 200  },
					new Company() { Id = 2, Country = "Canada", Name = "Corporate2",  YearFounded=1981, NumberOfEmployees= 300 },
					new Company() { Id = 3, Country = "USA", Name = "Corporate3", YearFounded=1980, NumberOfEmployees= 400 },
					new Company() { Id = 4, Country = "Canada", Name = "Corporate4", YearFounded=1983, NumberOfEmployees= 200},
				};
			Repository.InsertRange(_data);
			Repository.SaveAsync();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown() 
		{
			(Repository as CompaniesTestRepository).Dispose(true);
		}

		[TearDown]
		public async Task TearDown()
		{
			var count = await Repository.GetCountAsync();
			for (var i = 0; i < count; i++)
			{
				await Repository.DeleteRow(0);
				await Repository.SaveAsync();
			}
		}

		[Test]
		public async Task GenerateData_CheckCount() {
			var generatedDataCount = 5;
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == _data.Count);
			await Generate(generatedDataCount);
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == generatedDataCount + _data.Count);
		}


		private List<Company> _data = new List<Company>();
		[Test]
		public async Task ClearTable_CheckCount()
		{
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == _data.Count);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		[TestCase("Country", "id", "USA", 4)]
		[TestCase("Name", "NumberOfEmployees", "Corporate2", 300)]
		[TestCase("YearFounded", "id", "1980", 4)]
		[TestCase("Id", "YearFounded", "2", 1981)]
		[TestCase("NumberOfEmployees", "YearFounded", "200", 1980 + 1983)]
		public async Task FilterByOneParams_CheckSum(string filterColumn, string sumColumn, string value, int expected)
		{
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value });
			Assert.AreEqual(expected, actual);
		}


		[Test]
		[TestCase("Name", "NumberOfEmployees", "Corporate2", "Corporate3", 700)]
		[TestCase("YearFounded", "id", "1980", "1981", 6)]
		public async Task FilterByTwoParams_CheckSum(string filterColumn, string sumColumn, string value, string value1, int expected)
		{
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value, value1 });
			Assert.AreEqual(expected, actual);
		}
	}
}
