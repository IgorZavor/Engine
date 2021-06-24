using Engine.DAL.Models;
using Engine.Tests.Repositories.Users;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices.Users
{
	public class Tests : UsersTestService
	{

		public Tests() : base() {
		
		}

		private List<User> _data = new List<User>();

		[SetUp]
		public void SetUp()
		{
			_data =  new List<User> {
				new User() { Id = 1, Age = 32, Country = "USA", Name = "Kate", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Russia", Name = "Sam", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson2" },
			};
			Repository.InsertRange(_data);
			Repository.SaveAsync();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			(Repository as UsersTestRepository).Dispose(true);
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
			Assert.IsTrue(count == 9);
		}

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
		[TestCase("Country", "id", "Canada", 6)]
		[TestCase("Name", "Age", "Johny", 99 )]
		[TestCase("Surname", "id", "Jackson2", 7)]
		[TestCase("Id", "Age", "3", 89)]
		[TestCase("Age", "Age", "69", 69)]
		public async Task FilterByOneValue_CheckSum(string filterColumn, string sumColumn, string value, int expected)
		{
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value });
			Assert.AreEqual(expected, actual);
		}


		[Test]
		[TestCase("Country", "id", "USA", "Russia", 4)]
		[TestCase("Name", "Age", "Johny", "Sam", 89+99)]
		[TestCase("Surname", "id", "Jackson2", "Jackson", 9)]
		[TestCase("Id", "Age", "3", "4", 158)]
		[TestCase("Age", "Age", "69", "89", 158)]
		public async Task FilterByTwoValue_CheckSum(string filterColumn, string sumColumn, string value, string value1, int expected)
		{
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value1, value });
			Assert.AreEqual(expected, actual);
		}

	}
}
