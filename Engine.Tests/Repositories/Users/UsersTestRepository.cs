using Engine.DAL.Contexts;
using Engine.DAL.Repositories.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Engine.Tests.Repositories.Users
{
    public class UsersTestRepository : UsersRepository, IDisposable
    {
		private static SqliteConnection _connection;
		private static DbContextOptions<EngineContext> GetOptions()
		{
			_connection = new SqliteTestConnection().Connection;
			_connection.Open();
			var options = new DbContextOptionsBuilder<EngineContext>()
					.UseSqlite(_connection)
					.Options;
			return options;
		}

		public UsersTestRepository() : base(new EngineContext(GetOptions()))
        {
			_context.Database.EnsureCreated();
		}

		public void Dispose() 
		{
		}
	}
}
