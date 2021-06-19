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
		public async Task SetUp() {
			var companies = new List<Log> { 
				new Log() { Id = 1,  Author="Jack", DateTime="10.02.2021",Filter="" },
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter=""},
				new Log() { Id = 3, Author = "Samanta", DateTime="03.03.2021", Filter=""},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter=""},
			};
			InsertRange (companies);
			await SaveAsync();
		}

		[Test]
		public async Task AddLog_CheckNewEntity()
		{
			var logsCount = await GetCountAsync();
			var log = new Log() { Id = logsCount + 1,  };
			Insert(log);
			await SaveAsync();
			var count = await GetCountAsync();
			Assert.AreEqual(5, count);

			var newComapny = (await Get(logsCount)) as Log;
			Assert.IsNotNull(newComapny);
			AreEqualByJson(log, newComapny);
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

			var deletedCompany = await GetEntities().FirstOrDefaultAsync(u => (u as Log).Id == deletedId);
			Assert.IsNull(deletedCompany);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition() 
		{
			var row = 1;
			var log = (await GetEntities().ToListAsync())[row] as Log;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed= await GetEntities().FirstOrDefaultAsync(u => (u as Company).Id == log.Id);
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
