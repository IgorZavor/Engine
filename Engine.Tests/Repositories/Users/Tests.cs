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
		public async Task SetUp() 
		{
			var users = new List<User> { 
				new User() { Id = 1, Age = 32, Country = "USA", Name = "James", Surname = "Milner" },
				new User() { Id = 2, Age = 99, Country = "Canada", Name = "Johny", Surname = "Jackson" },
				new User() { Id = 3, Age = 89, Country = "Canada", Name = "Johny2", Surname = "Jackson2" },
				new User() { Id = 4, Age = 69, Country = "Canada", Name = "Johny1", Surname = "Jackson1" },
			};
			InsertRange (users);
			await SaveAsync();
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

			var addedUser = (await Get(userId)) as User;
			Assert.IsNotNull(addedUser);
			AreEqualByJson(user, addedUser);
		}

		[Test]
		public async Task RemoveUserById_CheckCount()
		{
			var DeletedId = 1;
			var countBefore = await GetCountAsync();
			Delete(DeletedId);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed = await GetEntities().FirstOrDefaultAsync(u => (u as User).Id == DeletedId);
			Assert.IsNull(removed);
		}

		[Test]
		public async Task RemoveRow_CheckTableCondition()
		{
			var row = 1;
			var userAtRow = (await GetEntities().ToListAsync())[row] as User;
			var countBefore = await GetCountAsync();
			await DeleteRow(row);
			await SaveAsync();
			var countAfter = await GetCountAsync();
			Assert.AreEqual(countBefore - 1, countAfter);

			var removed = await GetEntities().FirstOrDefaultAsync(u => (u as User).Id == userAtRow.Id);
			Assert.IsNull(removed);
		}


		public static void AreEqualByJson(object expected, object actual)
		{
			var expectedJson = JsonSerializer.Serialize(expected);
			var actualJson = JsonSerializer.Serialize(actual);
			Assert.AreEqual(expectedJson, actualJson);
		}
	}
}
