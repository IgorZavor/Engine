using Engine.DAL.Models;
using Engine.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.LogsServices.Logs
{
	public class Tests : LogsTestService
	{

		public Tests() : base() {}

		[Test]
		public async Task ClearTable_CheckCount()
		{
			// Fill data before cleaning
			var logs = new List<Log> {
				new Log() { Id = 1,  Author="Jack", DateTime="10.02.2021", Filter="" },
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter=""},
				new Log() { Id = 3, Author = "Samanta", DateTime="03.03.2021", Filter=""},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter=""},
			};
			Repository.InsertRange(logs);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		[TestCase("Author", "id", "USA")]
		[TestCase("Id", "sum", "Corporation")]
		[TestCase("DateTime", "id", "100")]
		[TestCase("Filter", "sum", "2")]
		[TestCase("Sum", "id", "2")]
		public async Task FilterBy_CheckSum(string filterColumn, string sumColumn, string filterValue)
		{
			var logs = new List<Log> {
				new Log() { Id = 1,  Author= "Jack", DateTime="10.02.2021", Filter="", Sum = 10 },
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter="filter1", Sum = 20},
				new Log() { Id = 3, Author = "Sam", DateTime="10.02.2021", Filter="", Sum = 30},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter="filter1", Sum = 10},
			};
			Repository.InsertRange(logs);
			await Repository.SaveAsync();
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);
			var filtered = FilterHelper.GetFilteredLogs(logs, filterColumn, new List<string> { filterValue });
			var expected = SumHelper.GetLogsSum(filtered, sumColumn);
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { filterValue });
			Assert.AreEqual(expected, actual);
		}
	}
}
