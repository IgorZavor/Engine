using Engine.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Engine.Tests.Repositories.Companies
{
	[TestFixture]
	public class Tests: CompaniesTestRepository
	{
		[SetUp]
		public void SetUp() {
			var companies = new List<Company> { 
				new Company() { Id = 1, Country = "USA", Name = "James Inc.",  NumberOfEmployees = 22, YearFounded = 1920   },
				new Company() { Id = 2, Country = "Canada", Name = "Johny Industry", NumberOfEmployees = 33, YearFounded = 1933},
				new Company() { Id = 3, Country = "Canada", Name = "Pepsi", NumberOfEmployees= 44, YearFounded = 1818},
				new Company() { Id = 4, Country = "Canada", Name = "Coca-Cola", NumberOfEmployees = 33, YearFounded = 1903},
			};
			InsertRange (companies);
			SaveAsync();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			Dispose(true);
		}

		[TearDown]
		public async Task TearDown()
		{
			var count = await GetCountAsync();
			for (var i = 0; i < count; i++)
			{
				await DeleteRow(0);
				await SaveAsync();
			}
		}



		[Test]
		public async Task AddCompany_CheckNewEntity()
		{
			var companyId = await GetCountAsync();
			var company = new Company() { Id = companyId + 1, Country = "Estonia", Name = "Richard's Paper", NumberOfEmployees = 12312, YearFounded = 1919 };
			Insert(company);
			await SaveAsync();
			var count = await GetCountAsync();
			Assert.AreEqual(5, count);

			var newComapny = (await GetEntity(companyId + 1)) as Company;
			Assert.IsNotNull(newComapny);
			AreEqualByJson(company, newComapny);
		}

		[Test]
		public async Task RemoveCompanyById_CheckCount()
		{
			var deletedId = 1;
			var countBefore = await GetCountAsync();
			Delete(deletedId);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var deletedCompany = await GetEntity(deletedId);
			Assert.IsNull(deletedCompany);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition() 
		{
			var row = 1;
			var comapny = (await GetEntities())[row] as Company;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed = await GetEntity(comapny.Id);
			Assert.IsNull(removed);
		}

		[Test]
		public async Task FilterBy_CheckResult()
		{
			var results = await FilterBy("Country", new List<string> { "USA" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var company = r as Company;
				Assert.AreEqual("USA", company.Country);
			}

			results = await FilterBy("Name", new List<string> { "Pepsi" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var company = r as Company;
				Assert.AreEqual("Pepsi", company.Name);
			}

			results = await FilterBy("NumberOfEmployees", new List<string> { "33" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results)
			{
				var company = r as Company;
				Assert.AreEqual(33, company.NumberOfEmployees);
			}

			results = await FilterBy("YearFounded", new List<string> { "1818", "1933" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results)
			{
				var company = r as Company;

				Assert.IsTrue(company.YearFounded == 1818 || company.YearFounded == 1933);
			}

			results = await FilterBy("id", new List<string> { "2", "3" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results)
			{
				var company = r as Company;

				Assert.IsTrue(company.Id == 2 || company.Id == 3);
			}
		}

		public static void AreEqualByJson(object expected, object actual)
		{
			var expectedJson = JsonConvert.SerializeObject(expected);
			var actualJson = JsonConvert.SerializeObject(actual);
			Assert.AreEqual(expectedJson, actualJson);
		}
	}
}
