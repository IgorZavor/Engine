using Engine.DAL.Models;
using Engine.Tests.Repositories.Logs;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.LogsServices.Logs
{
	public class Tests : LogsTestService
	{

		public Tests() : base() {}


		[SetUp]
		public void SetUp()
		{
			var logs = new List<Log> {
				new Log() { Id = 1,  Author= "Jack", DateTime="10.02.2021", Filter="", Sum = 10 },
				new Log() { Id = 2, Author = "Sam",  DateTime="03.02.2021", Filter="filter1", Sum = 20},
				new Log() { Id = 3, Author = "Sam", DateTime="10.02.2021", Filter="", Sum = 30},
				new Log() { Id = 4, Author = "July",  DateTime="04.06.2021", Filter="filter1", Sum = 10},
			};
			Repository.InsertRange(logs);
			Repository.SaveAsync();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			(Repository as LogsTestRepository).Dispose(true);
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
		public async Task ClearTable_CheckCount()
		{
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		[TestCase("Author", "id", "dda", 0)]
		[TestCase("Id", "sum", "1", 10)]
		[TestCase("DateTime", "id", "03.02.2021", 2 )]
		[TestCase("Filter", "sum", "filter1", 30)]
		[TestCase("Sum", "id", "10", 5)]
		public async Task FilterBy_CheckSum(string filterColumn, string sumColumn, string filterValue, int expected)
		{
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { filterValue });
			Assert.AreEqual(expected, actual);
		}
	}
}
