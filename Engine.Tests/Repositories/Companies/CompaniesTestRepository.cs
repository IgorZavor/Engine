using Engine.DAL.Contexts;
using Engine.DAL.Repositories.Companies;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Engine.Tests.Repositories.Companies
{
    public class CompaniesTestRepository : CompaniesRepository, IDisposable
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
		public CompaniesTestRepository() : base(new EngineContext(GetOptions()))
        {
			_context.Database.EnsureCreated();
		}

		public void Dispose(bool disposeBase = false)
		{
			if (disposeBase) 
			{
				base.Dispose();
			}
		}
	}
}
