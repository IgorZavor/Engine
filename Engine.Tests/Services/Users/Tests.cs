using Engine.DAL.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Tests.Services.Users
{
	public class Tests : UsersTestService
	{

		public Tests() : base() {
		
		}

		[SetUp]
		public async Task SetUp()
		{
			var users = new List<User> {
				new User() { Id = 1, Age = 32, Country = "USA", Name = "Kate", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Sam", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson2" },
			};
			Repository.InsertRange(users);
			await Repository.SaveAsync();
		}

		[Test]
		public async Task GenerateData_CheckCount() {
			var generatedDataCount = 5;
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);
			await Generate(generatedDataCount);
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == generatedDataCount);
		}

		[Test]
		public async Task ClearTable_CheckCount()
		{
			// Fill data before cleaning
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		public async Task FilterBy_CheckSum(string sumColumn)
		{
			var filtersDictionary = new List<FIlterResult>
			{
				new FIlterResult { FilterColumn = "Country", FilterValues = new List<string> { "Canada" }, Sum = 99 + 89+ 69 , SumColumn="Age" },
				new FIlterResult { FilterColumn =  "Name", FilterValues= new List<string> { "Kate", "Sam" }, Sum = 89 + 32, SumColumn = "Age" },
				new FIlterResult { FilterColumn =  "Surname", FilterValues= new List<string> { "Jackson2" }, Sum = 7, SumColumn = "Id" },
				new FIlterResult { FilterColumn =  "Age", FilterValues= new List<string> { "89" }, Sum = 3, SumColumn = "Id" },
			};


			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			var i = 0;
			foreach (var filter in filtersDictionary)
			{
				var result = await FilterBy(filter.FilterColumn, sumColumn, filter.FilterValues);
				Assert.AreEqual(filter.Sum, result);
				i++;
			}
		}


		private class FIlterResult 
		{
			public int Sum { get; set; }
			public string SumColumn { get; set; }
			public string FilterColumn { get; set; }
			public List<string> FilterValues { get; set; }
		}
	}
}
