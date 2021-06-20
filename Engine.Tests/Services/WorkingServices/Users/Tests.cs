using Engine.DAL.Models;
using Engine.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices.Users
{
	public class Tests : UsersTestService
	{

		public Tests() : base() {
		
		}

		[Test]
		public async Task GenerateData_CheckCount() {
			var generatedDataCount = 5;
			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
			var users = new List<User> {
				new User() { Id = 1, Age = 32, Country = "USA", Name = "Kate", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Sam", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson2" },
			};
			Repository.InsertRange(users);
			await Repository.SaveAsync();

			await Generate(generatedDataCount);
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 9);
		}

		[Test]
		public async Task ClearTable_CheckCount()
		{
			// Fill data before cleaning
			var users = new List<User> {
				new User() { Id = 1, Age = 32, Country = "USA", Name = "Kate", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Sam", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson2" },
			};
			Repository.InsertRange(users);
			await Repository.SaveAsync();

			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			await Clear();
			count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 0);
		}

		[Test]
		[TestCase("Country", "id", "USA")]
		[TestCase("Name", "Age", "Corporate2", "Corporate3")]
		[TestCase("Surname", "id", "Jackson2")]
		[TestCase("Id", "Age", "3")]
		[TestCase("Age", "Age", "69")]
		public async Task FilterBy_CheckSum(string filterColumn, string sumColumn, string value, string? value1 = "")
		{
			var users = new List<User> {
				new User() { Id = 1, Age = 32, Country = "USA", Name = "Kate", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Sam", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson2" },
			};
			Repository.InsertRange(users);
			await Repository.SaveAsync();

			var count = await Repository.GetCountAsync();
			Assert.IsTrue(count == 4);

			var filtered = FilterHelper.GetFilteredUsers(users, filterColumn, new List<string> { value1, value });
			var expected = SumHelper.GetUsersSum(filtered, sumColumn);
			var actual = await FilterBy(filterColumn, sumColumn, new List<string> { value });
			Assert.AreEqual(expected, actual);
		}

	}
}
