using Microsoft.Data.Sqlite;


namespace Engine.Tests.Repositories
{
	public class SqliteTestConnection
	{
		private const string InMemoryConnectionString = "DataSource=:memory:";
		private readonly SqliteConnection _connection;

		public  SqliteConnection Connection { get { return _connection; } }
		public SqliteTestConnection() {
			_connection = new SqliteConnection(InMemoryConnectionString);
		}
	}
}
