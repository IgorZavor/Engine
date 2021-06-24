using Engine.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Engine.Tests.Repositories.Logs
{
	[TestFixture]
	public class Tests: LogsTestRepository
	{
		[SetUp]
		public void SetUp() {
			var data = new List<Log> { 
				new Log() { Id = 1,  Author="Jack", DateTime="10.02.2021",Filter="", Sum = 10},
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter="filter2", Sum = 10},
				new Log() { Id = 3, Author = "Sam", DateTime="03.03.2021", Filter="filter1", Sum = 30},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter="", Sum = 10},
			};
			InsertRange (data);
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
		public async Task AddLog_CheckNewEntity()
		{
			var logsCount = await GetCountAsync();
			var log = new Log() { Id = 5, Author = "July", DateTime = "04.06.2021", Filter = "", Sum = 12 };
			Insert(log);
			await SaveAsync();
			var count = await GetCountAsync();
			Assert.AreEqual(5, count);

			var newLog = (await GetEntity(5)) as Log;
			Assert.IsNotNull(newLog);
			AreEqualByJson(log, newLog);
		}

		[Test]
		public async Task RemoveLogById_CheckCount()
		{
			var deletedId = 1;
			var countBefore = await GetCountAsync();
			Delete(deletedId);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var deletedLog= await GetEntity(deletedId);
			Assert.IsNull(deletedLog);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition() 
		{
			var row = 1;
			var log = (await GetEntities())[row] as Log;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed= await GetEntity(log.Id);
			Assert.IsNull(removed);
		}

		[Test]
		public async Task FilterBy_CheckResult()
		{
			var results = await FilterBy("Author", new List<string> { "Sam" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results) 
			{
				var log = r as Log;
				Assert.AreEqual("Sam", log.Author);
			}

			results = await FilterBy("Sum", new List<string> { "10" });
			Assert.AreEqual(3, results.Count);
			foreach (var r in results)
			{
				var log = r as Log;
				Assert.AreEqual(10, log.Sum);
			}

			results = await FilterBy("id", new List<string> { "2" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var log = r as Log;
				Assert.AreEqual(2, log.Id);
			}

			results = await FilterBy("filter", new List<string> { "filter2", "filter1" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results)
			{
				var log = r as Log;

				Assert.IsTrue(log.Filter == "filter2" || log.Filter == "filter1");
			}

			results = await FilterBy("datetime", new List<string> { "03.03.2021" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var log = r as Log;

				Assert.IsTrue(log.DateTime == "03.03.2021");
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
