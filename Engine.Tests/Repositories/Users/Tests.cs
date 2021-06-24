using Engine.DAL.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;

namespace Engine.Tests.Repositories.Users
{
	[TestFixture]
	public class Tests: UsersTestRepository
	{
		[SetUp]
		public void SetUp() 
		{
			var users = new List<User> { 
				new User() { Id = 1, Age = 32, Country = "USA", Name = "James", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Johny2", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson1" },
			};
			InsertRange (users);
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
		public async Task AddUser_CheckNewEntity()
		{
			var userId = 5;
			var user = new User() { Id = userId, Age = 43, Country = "Estonia", Name = "Richard", Surname = "Goldway" };
			Insert(user);
			await SaveAsync();
			var count = await GetCountAsync();
			Assert.AreEqual(5, count);

			var addedUser = (await GetEntity(userId)) as User;
			Assert.IsNotNull(addedUser);
			AreEqualByJson(user, addedUser);
		}

		[Test]
		public async Task RemoveUserById_CheckCount()
		{
			var deletedId = 1;
			var countBefore = await GetCountAsync();
			Delete(deletedId);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed = await GetEntity(deletedId);
			Assert.IsNull(removed);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition()
		{
			var row = 1;
			var userAtRow = (await GetEntities())[row] as User;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed = await GetEntity(userAtRow.Id);
			Assert.IsNull(removed);
		}

		[Test]
		public async Task FilterBy_CheckResult()
		{
			var results = await FilterBy("Age", new List<string> { "99" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var user = r as User;
				Assert.AreEqual(99, user.Age);
			}

			results = await FilterBy("Country", new List<string> { "Canada" });
			Assert.AreEqual(3, results.Count);
			foreach (var r in results)
			{
				var user = r as User;
				Assert.AreEqual("Canada", user.Country);
			}

			results = await FilterBy("Name", new List<string> { "Johny" });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var user = r as User;
				Assert.AreEqual("Johny", user.Name);
			}

			results = await FilterBy("Surname", new List<string> { "Jackson", "Jackson1" });
			Assert.AreEqual(2, results.Count);
			foreach (var r in results)
			{
				var user = r as User;

				Assert.IsTrue(user.Surname == "Jackson" || user.Surname == "Jackson1");
			}

			results = await FilterBy("id", new List<string> { "3",  });
			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				var user = r as User;

				Assert.IsTrue(user.Id == 3);
			}
		}


		public static void AreEqualByJson(object expected, object actual)
		{
			var expectedJson = JsonSerializer.Serialize(expected);
			var actualJson = JsonSerializer.Serialize(actual);
			Assert.AreEqual(expectedJson, actualJson);
		}
	}
}
