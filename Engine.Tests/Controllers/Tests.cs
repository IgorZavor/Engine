using Engine.Models.In;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using Engine.DAL.Models;
using Engine.Cache;
using Newtonsoft.Json;
using Engine.Models.Out;
using Microsoft.AspNetCore.Mvc;
using Engine.Services.WorkingServices;
using Engine.Services.WorkingServices.Companies;
using Engine.Services.WorkingServices.Users;
using Engine.Services.LogsServices.Logs;
using Engine.Services;
using Engine.Services.LogsServices;
using Engine.DAL.Repositories;

namespace Engine.Tests.Controllers
{
	[TestFixture]
	public class Tests: EngineTestController
	{
		private WorkingBaseService _workingService;
		private IBaseService _service;

		private WorkingBaseService WorkingServiceResolver(Tables tableType) {
			if (_workingService != null) 
			{
				return _workingService;
			}
			switch (tableType)
			{
				case Tables.Companies:
					return _workingService = new CompaniesTestService();
				default:
					return _workingService = new UsersTestService();
			}
		}

		private IBaseService ServiceResolver(Tables tableType)
		{
			if (_service != null) 
			{
				return _service;
			}
			switch (tableType)
			{
				case Tables.Companies:
					return _service= new CompaniesTestService();
				case Tables.Logs:
					return _service = new LogsTestService();
				default:
					return _service= new UsersTestService();
			}
		}


		[Test]
		[TestCase("Users", 5)]
		[TestCase("Companies", 10)]
		public async Task Generate_CheckResult(string name, int count)
		{
			var model = new GenerateModel() { Count = count, Table = name };
			var result = ((await Generate(model, WorkingServiceResolver)) as JsonResult).Value as ResultOut;
			Assert.IsFalse(result.Error);
			var rowCount = await _workingService.Repository.GetCountAsync();
			Assert.IsTrue(rowCount == count);
			
		}

		[Test]
		[TestCase("Used", 5)]
		[TestCase("Comp", 10)]
		public async Task GenerateFailed_CheckResult(string name, int count)
		{
			var model = new GenerateModel() { Count = count, Table = name };
			var result = ((await Generate(model, WorkingServiceResolver)) as JsonResult).Value as ResultOut;
			Assert.IsTrue(result.Error);
			Assert.IsTrue(_service == null);
		}

		[Test]
		public async Task ClearUsers_CheckEntitiesCount()
		{
			var service = ServiceResolver(Tables.Users) as WorkingBaseService;
			var repo = service.Repository;
			var rowsCount = await service.Repository.GetCountAsync();
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
			rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == users.Count);


			var clearModel = new ClearModel() { Table = "Users" };
			var result  = (await Clear(clearModel, ServiceResolver) as JsonResult).Value as ResultOut;
			Assert.IsFalse(result.Error);
			rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			
		}

		[Test]
		public async Task ClearCompanies_CheckEntitiesCount()
		{
			var service = ServiceResolver(Tables.Companies) as WorkingBaseService;
			var repo = service.Repository;
			var rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			var data = new List<Company>()
			{
				new Company{ Id= 1, YearFounded=1940, Country="USA", Name="Corporation", NumberOfEmployees= 1010 },
				new Company{ Id= 2, YearFounded=1902, Country="Russia", Name="Corporation", NumberOfEmployees=100 },
				new Company{ Id= 3, YearFounded=2021, Country="Brazil", Name="Corporation", NumberOfEmployees=242 },
				new Company{ Id= 4, YearFounded=2020, Country="Canada", Name="Corporation", NumberOfEmployees=392 }
			};

			repo.InsertRange(data);
			await repo.SaveAsync();
			rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == data.Count);

			var clearModel = new ClearModel() { Table = "Companies" };
			await Clear(clearModel, ServiceResolver);
			rowsCount = await service.Repository.GetCountAsync(); ;
			Assert.IsTrue(rowsCount == 0);
			
		}

		[Test]
		public async Task ClearLogs_CheckEntitiesCount()
		{
			var service = ServiceResolver(Tables.Logs) as LogsServiceBase;
			var repo = service.Repository;
			var rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == 0);
			var data = new List<Log> {
				new Log() { Id = 1,  Author="Jack", DateTime="10.02.2021",Filter="", Sum= 0},
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter="", Sum= 0},
				new Log() { Id = 3, Author = "Samanta", DateTime="03.03.2021", Filter="", Sum= 0},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter="", Sum= 0},
			};

			repo.InsertRange(data);
			await repo.SaveAsync();
			rowsCount = await service.Repository.GetCountAsync();
			Assert.IsTrue(rowsCount == data.Count);

			var clearModel = new ClearModel() { Table = "Logs" };
			await Clear(clearModel, ServiceResolver);
			rowsCount = await service.Repository.GetCountAsync(); ;
			Assert.IsTrue(rowsCount == 0);
			
		}

		[Test]
		[TestCase("Country", "NumberOfEmployees", "USA", 1252)]
		[TestCase("Name", "YearFounded", "Corporation", 4041)]
		[TestCase("NumberOfEmployees", "NumberOfEmployees", "100", 100)]
		[TestCase("id", "NumberOfEmployees", "2", 100)]
		public async Task Companies_FilterAndSum_CheckSum(string filterColumn, string sumColumn, string filterValue, int expected)
		{
			var repo = (ServiceResolver(Tables.Companies) as WorkingBaseService).Repository;
			await FillCompanies(repo);

			var model = new FilterByModel() {
				Table= "Companies", 
				Author="Sam", 
				FilterColumn= filterColumn, 
				SummaryColumn = sumColumn, 
				Filters = new List<Filter> { new Filter { Value = filterValue } } 
			};
			var result = (await FilterAndSum(model, ServiceResolver)) as JsonResult;
			var sumOut = result.Value as SumOut;
			Assert.IsFalse(sumOut. Error);
			Assert.AreEqual(expected, sumOut.Sum);
			var cacheModel = TryGetValueFromCache(CacheKeys.FilterBy);
			Assert.AreEqual(model.Author, cacheModel.Author);
			Assert.AreEqual(GetFilter(model.FilterColumn, model.Filters), cacheModel.Filter);
			Assert.AreEqual(expected, cacheModel.Sum);
			
		}


		private async Task FillCompanies(IRepository repo) 
		{
			var count = await repo.GetCountAsync();
			if (count == 0)
			{
				var companies = new List<Company>()
			{
				new Company{ Id= 1, YearFounded=1940, Country="USA", Name="Corporation2", NumberOfEmployees= 1010 },
				new Company{ Id= 2, YearFounded=1902, Country="Russia", Name="Corporation3", NumberOfEmployees=100 },
				new Company{ Id= 3, YearFounded=2021, Country="USA", Name="Corporation", NumberOfEmployees=242 },
				new Company{ Id= 4, YearFounded=2020, Country="Canada", Name="Corporation", NumberOfEmployees=392 }
			};

				repo.InsertRange(companies);
				await repo.SaveAsync();
			}
		}

		[Test]
		[TestCase("Country2", "NumberOfEmployees", "USA")]
		[TestCase("Name1", "YearFounded", "Corporation")]
		[TestCase("NumberOfEmployees3", "NumberOfEmployees", "100")]
		[TestCase("id", "NumberOfEmployees4", "2")]
		public async Task Companies_FilterAndSumError_CheckResult(string filterColumn, string sumColumn, string filterValue)
		{
			var repo = (ServiceResolver(Tables.Companies) as WorkingBaseService).Repository;
			await FillCompanies(repo);

			var model = new FilterByModel()
			{
				Table = "Companies",
				Author = "Sam",
				FilterColumn = filterColumn,
				SummaryColumn = sumColumn,
				Filters = new List<Filter> { new Filter { Value = filterValue } }
			};
			var result = (await FilterAndSum(model, ServiceResolver));
			var sumOut = (result as JsonResult).Value as SumOut;
			Assert.IsTrue(sumOut.Error);
		}


		[Test]
		[TestCase("Country", "id", "Brazil", 4)]
		[TestCase("Name", "age", "Mat", 84)]
		[TestCase("Surname", "age", "Halpert1", 42)]
		[TestCase("Age", "age", "42", 42)]
		[TestCase("Id", "age", "2", 42)]
		public async Task Users_FilterAndSum_CheckSumAndMemoryCacheValue(string filterColumn, string sumColumn, string filterValue1, int expected)
		{
			var repo = (ServiceResolver(Tables.Users) as WorkingBaseService).Repository;
			await FillUsers(repo);

			var model = new FilterByModel() { 
				Table ="Users",
				Author = "Sam", FilterColumn = filterColumn,
				SummaryColumn = sumColumn, 
				Filters = new List<Filter> { new Filter { Value = filterValue1 } } 
			};

			var result = ((await FilterAndSum(model, ServiceResolver)) as JsonResult);
			var sumOut = result.Value as SumOut;
			Assert.IsFalse(sumOut.Error);
			Assert.AreEqual(expected, sumOut.Sum);
			var cacheModel = TryGetValueFromCache(CacheKeys.FilterBy);
			Assert.AreEqual(model.Author, cacheModel.Author);
			Assert.AreEqual(GetFilter(model.FilterColumn, model.Filters), cacheModel.Filter);
			Assert.AreEqual(expected, cacheModel.Sum);
			
		}

		[Test]
		[TestCase("Country2", "id", "Brazil")]
		[TestCase("Name", "age2", "Mat")]
		[TestCase("Sur3name", "age", "Halpert1")]
		[TestCase("Ag4e", "age", "42")]
		[TestCase("Id", "age5", "2")]
		public async Task Users_FilterAndSumError_CheckResult(string filterColumn, string sumColumn, string filterValue1)
		{
			var repo = (ServiceResolver(Tables.Users) as WorkingBaseService).Repository;
			await FillUsers(repo);


			var model = new FilterByModel()
			{
				Table = "Users",
				Author = "Sam",
				FilterColumn = filterColumn,
				SummaryColumn = sumColumn,
				Filters = new List<Filter> { new Filter { Value = filterValue1 }}
			};

			var result = (await FilterAndSum(model, ServiceResolver));
			var sumOut = (result as JsonResult).Value as SumOut;
			Assert.IsTrue(sumOut.Error);
		}

        private async Task FillUsers(IRepository repo)
        {
            var count = await repo.GetCountAsync();
            if (count == 0)
            {
                var users = new List<User>()
            {
                new User{ Id= 1, Age=22, Country="Brazil", Name="Jim", Surname="Halpert" },
                new User{ Id= 2, Age=42, Country="Russia", Name="Scott", Surname="Halpert1" },
                new User{ Id= 3, Age=32, Country="Brazil", Name="Mat", Surname="Halpert2" },
                new User{ Id= 4, Age=52, Country="Canada", Name="Mat", Surname="Halpert3" }
            };
                repo.InsertRange(users);
                await repo.SaveAsync();
            }
        }


        [Test]
		[TestCase("Filter", "id", "filter1", 1)]
		[TestCase("Author", "sum", "Samanta", 10)]
		[TestCase("DateTime", "sum", "03.03.2021", 10)]
		[TestCase("Sum", "id", "10", 4)]
		[TestCase("id", "id", "1", 1)]
		public async Task Logs_FilterAndSum_CheckSumAndMemoryCacheValue(string filterColumn, string sumColumn, string filterValue, int expected)
		{
			var repo = (ServiceResolver(Tables.Logs) as LogsServiceBase).Repository;
			await FillLogs(repo);

			var model = new FilterByModel() { 
				Table = "Logs", 
				Author = "Sam",
				FilterColumn = filterColumn, SummaryColumn = sumColumn, 
				Filters = new List<Filter> { new Filter { Value = filterValue } } };

			var result = ((await FilterAndSum(model, ServiceResolver)) as JsonResult);
			var sumOut = result.Value as SumOut;
			Assert.IsFalse(sumOut.Error);
			Assert.AreEqual(expected, sumOut.Sum);
			var cacheModel = TryGetValueFromCache(CacheKeys.FilterBy);
			Assert.AreEqual(model.Author, cacheModel.Author);
			Assert.AreEqual(GetFilter(model.FilterColumn, model.Filters), cacheModel.Filter);
			Assert.AreEqual(expected, cacheModel.Sum);
		}

		[Test]
		[TestCase("Filter", "i2d", "filter1")]
		[TestCase("Author3", "sum", "Samanta")]
		[TestCase("DateTime", "sum3", "03.03.2021")]
		[TestCase("Sum2", "id", "10")]
		[TestCase("id", "i2d", "1")]
		public async Task Logs_FilterAndSumError_CheckResult(string filterColumn, string sumColumn, string filterValue)
		{
			var repo = (ServiceResolver(Tables.Logs) as LogsServiceBase).Repository;
			await FillLogs(repo);

			var model = new FilterByModel()
			{
				Table = "Logs",
				Author = "Sam",
				FilterColumn = filterColumn,
				SummaryColumn = sumColumn,
				Filters = new List<Filter> { new Filter { Value = filterValue } }
			};

			var result = (await FilterAndSum(model, ServiceResolver));
			var sumOut = (result as JsonResult).Value as SumOut;
			Assert.IsTrue(sumOut.Error);
		}

		private async Task FillLogs(IRepository repo) 
		{
			var count = await repo.GetCountAsync();
			if (count == 0)
			{
				var data = new List<Log> {
				new Log() { Id = 1,  Author="Jack", DateTime="10.02.2021",Filter="filter1", Sum = 10},
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter="", Sum = 20},
				new Log() { Id = 3, Author = "Samanta", DateTime="03.03.2021", Filter="", Sum = 10},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter="filter2", Sum = 22},
			};
				repo.InsertRange(data);
				await repo.SaveAsync();
			}
		}

		[OneTimeTearDown]
		public  void Dispose()
		{
			if (_workingService != null)
			{
				_workingService.Repository.Dispose();
				_workingService = null;
			}
			if (_service != null)
			{
				var logService = _service as LogsServiceBase;
				if (logService != null) 
				{
					logService.Repository.Dispose();
				}
				var workingBaseService = _service as WorkingBaseService;
				if (workingBaseService != null) 
				{
					workingBaseService.Repository.Dispose();
				}
				
				_service = null;
			}
		}

	}
}
