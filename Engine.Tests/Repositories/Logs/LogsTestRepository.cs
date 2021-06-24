using Engine.DAL.Contexts;
using Engine.DAL.Repositories.Logs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Engine.Tests.Repositories.Logs
{
    public class LogsTestRepository : LogsRepository
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

		public LogsTestRepository() : base(new EngineContext(GetOptions()))
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
