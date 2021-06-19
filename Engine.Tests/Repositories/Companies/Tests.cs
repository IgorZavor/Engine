using Engine.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Engine.Tests.Repositories.Companies
{
	[TestFixture]
	public class Tests: CompaniesTestRepository
	{
		[SetUp]
		public async Task SetUp() {
			var companies = new List<Company> { 
				new Company() { Id = 1, Country = "USA", Name = "James Inc.",  NumberOfEmployees = 22, YearFounded = 1920   },
				new Company() { Id = 2, Country = "Canada", Name = "Johny Industry", NumberOfEmployees = 33, YearFounded = 1933},
				new Company() { Id = 3, Country = "Canada", Name = "Pepsi", NumberOfEmployees= 44, YearFounded = 1818},
				new Company() { Id = 4, Country = "Canada", Name = "Coca-Cola", NumberOfEmployees = 55, YearFounded = 1903},
			};
			InsertRange (companies);
			await SaveAsync();
		}

		[Test]
		public async Task AddCompany_CheckNewEntity()
		{
			var companyId = await GetCountAsync();
			var company = new Company() { Id = companyId + 1, Country = "Estonia", Name = "Richard's Paper", NumberOfEmployees = 33, YearFounded = 1919 };
			Insert(company);
			await SaveAsync();
			var count = await GetCountAsync();
			Assert.AreEqual(5, count);

			var newComapny = (await Get(companyId)) as Company;
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

			var deletedCompany = await GetEntities().FirstOrDefaultAsync(u => (u as Company).Id == deletedId);
			Assert.IsNull(deletedCompany);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition() 
		{
			var row = 1;
			var comapny = (await GetEntities().ToListAsync())[row] as Company;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed= await GetEntities().FirstOrDefaultAsync(u => (u as Company).Id == comapny.Id);
			Assert.IsNull(removed);
		}

		public static void AreEqualByJson(object expected, object actual)
		{
			var expectedJson = JsonConvert.SerializeObject(expected);
			var actualJson = JsonConvert.SerializeObject(actual);
			Assert.AreEqual(expectedJson, actualJson);
		}
	}
}
