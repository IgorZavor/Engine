using Engine.Models.In;
using NUnit.Framework;
using System.Threading.Tasks;
using Engine.Services;
using Engine.Tests.Services.Users;
using Engine.Tests.Services.Companies;
using System.Collections.Generic;
using Engine.DAL.Models;
using System.Linq;
using Engine.Cache;
using Newtonsoft.Json;
using Engine.Models.Out;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace Engine.Tests.Controllers
{
	[TestFixture]
	public class Tests: EngineTestController
	{
		private ILogsService _service;

		private ILogsService ServiceResolver(Enums.Tables tableType) {
			if (_service != null)
				return _service;
			switch (tableType)
			{
				case Enums.Tables.Companies:
					return _service = new CompaniesTestService();
				default:
					return _service = new UsersTestService();
			}
		}


		[Test]
		[TestCase("Users", 5)]
		[TestCase("Companies", 10)]
		public async Task Generate_CheckResult(string name, int count)
		{
			var model = new GenerateIn() { Count = count, Table = name };
			var result = ((await Generate(model, ServiceResolver)) as JsonResult).Value as ResultOut;
			Assert.IsFalse(result.Error);
			var rowCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowCount == count);
			Dispose();
		}

		[Test]
		[TestCase("Used", 5)]
		[TestCase("Comp", 10)]
		public async Task GenerateFailed_CheckResult(string name, int count)
		{
			var model = new GenerateIn() { Count = count, Table = name };
			var result = ((await Generate(model, ServiceResolver)) as JsonResult).Value as ResultOut;
			Assert.IsTrue(result.Error);
			Assert.IsTrue(_service == null);
		}

		[Test]
		public async Task ClearUsers_CheckEntitiesCount()
		{
			var repo = ServiceResolver(Enums.Tables.Users).Repository;
			var rowsCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			var users = new List<User>()
			{
				new User{ Id= 1, Age=22, Country="USA", Name="Jim", Surname="Halpert" },
				new User{ Id= 2, Age=42, Country="Russia", Name="Scott", Surname="Halpert1" },
				new User{ Id= 3, Age=32, Country="Brazil", Name="Mat", Surname="Halpert2" },
				new User{ Id= 4, Age=52, Country="Canada", Name="Kellen", Surname="Halpert3" }
			};
			
			repo.InsertRange(users);
			await repo.SaveAsync();
			rowsCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == users.Count);


			var clearModel = new ClearIn() { Table = "Users" };
			var result  = (await Clear(clearModel, ServiceResolver) as JsonResult).Value as ResultOut;
			Assert.IsFalse(result.Error);
			rowsCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			Dispose();
		}

		[Test]
		public async Task ClearCompanies_CheckEntitiesCount()
		{
			var repo = ServiceResolver(Enums.Tables.Companies).Repository;
			var rowsCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			var users = new List<Company>()
			{
				new Company{ Id= 1, YearFounded=1940, Country="USA", Name="Corporation", NumberOfEmployees= 1010 },
				new Company{ Id= 2, YearFounded=1902, Country="Russia", Name="Corporation", NumberOfEmployees=100 },
				new Company{ Id= 3, YearFounded=2021, Country="Brazil", Name="Corporation", NumberOfEmployees=242 },
				new Company{ Id= 4, YearFounded=2020, Country="Canada", Name="Corporation", NumberOfEmployees=392 }
			};

			repo.InsertRange(users);
			await repo.SaveAsync();
			rowsCount = await _service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == users.Count);

			var clearModel = new ClearIn() { Table = "Companies" };
			await Clear(clearModel, ServiceResolver);
			rowsCount = await _service.Repository.GetCountAsync(); ;
			Assert.IsTrue(rowsCount == 0);
			Dispose();
		}

		[Test]
		[TestCase("Country", "NumberOfEmployees", "USA")]
		[TestCase("Name", "YearFounded", "Corporation")]
		public async Task  Companies_FilterAndSum_CheckSum(string filterColumn, string sumColumn, string value)
		{
			var repo = ServiceResolver(Enums.Tables.Companies).Repository;
			var companies = new List<Company>()
			{
				new Company{ Id= 1, YearFounded=1940, Country="USA", Name="Corporation2", NumberOfEmployees= 1010 },
				new Company{ Id= 2, YearFounded=1902, Country="Russia", Name="Corporation3", NumberOfEmployees=100 },
				new Company{ Id= 3, YearFounded=2021, Country="USA", Name="Corporation", NumberOfEmployees=242 },
				new Company{ Id= 4, YearFounded=2020, Country="Canada", Name="Corporation", NumberOfEmployees=392 }
			};

			repo.InsertRange(companies);
			await repo.SaveAsync();
			var expected = 0;
			if (filterColumn.ToLower() == "country") 
			{
				expected = companies.Where(c => c.Country == value).Select(c => c.NumberOfEmployees).Sum();
			}
			else if (filterColumn.ToLower() == "name") 
			{
				expected = companies.Where(c => c.Name == value).Select(c => c.NumberOfEmployees).Sum();
			}
				
			var model = new FilterByIn() {Table= "Companies", Author="Sam", FilterColumn= filterColumn,  SummaryColumn = sumColumn, Filters = new List<Filter> { new Filter { Value = value } } };
			var actual = await FilterAndSum(model, ServiceResolver);
			Assert.AreEqual(expected, actual);
			Dispose();
		}

		[Test]
		[TestCase("Country", "id", "Brazil")]
		[TestCase("Name", "age", "Mat")]
		public async Task Users_FilterAndSum_CheckSumAndMemoryCacheValue(string filterColumn, string sumColumn, string value)
		{
			var repo = ServiceResolver(Enums.Tables.Users).Repository;
			var users = new List<User>()
			{
				new User{ Id= 1, Age=22, Country="Brazil", Name="Jim", Surname="Halpert" },
				new User{ Id= 2, Age=42, Country="Russia", Name="Scott", Surname="Halpert1" },
				new User{ Id= 3, Age=32, Country="Brazil", Name="Mat", Surname="Halpert2" },
				new User{ Id= 4, Age=52, Country="Canada", Name="Mat", Surname="Halpert3" }
			};
			repo.InsertRange(users);
			await repo.SaveAsync();

			IEnumerable<User> filtered = null;
			if (filterColumn.ToLower() == "country")
			{
				filtered = users.Where(c => c.Country == value);
			}
			else if (filterColumn.ToLower() == "name") 
			{
				filtered = users.Where(c => c.Name == value);
			}

			var expected = 0;

			if (sumColumn.ToLower() == "age")
			{
				expected = filtered.Select(f => f.Age).Sum();
			}
			else if (sumColumn.ToLower() == "id") 
			{
				expected = filtered.Select(f => f.Id).Sum();
			}

			var model = new FilterByIn() { Table ="Users", Author = "Sam", FilterColumn = filterColumn, SummaryColumn = sumColumn,  Filters = new List<Filter> { new Filter { Value = value } } };
			var result = (await FilterAndSum(model, ServiceResolver)).ToString();
			var actual = JsonConvert.DeserializeObject<SumOut>(result ?? "");
			Assert.AreEqual(expected, actual.Sum);
			var cacheModel = TryGetValueFromCache(CacheKeys.FilterBy);
			Assert.AreEqual(model.Author, cacheModel.Author);
			Assert.AreEqual(GetFilter(model.FilterColumn, model.Filters), cacheModel.ColumnFilter);
			Assert.AreEqual(expected, cacheModel.Result);
			Dispose();
		}

		public  void Dispose()
		{
			_service.Repository.Dispose();
			_service = null;
		}

	}
}
