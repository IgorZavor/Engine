using Engine.DAL.Contexts;
using Engine.DAL.Repositories;
using Engine.DAL.Repositories.Companies;
using Engine.DAL.Repositories.Logs;
using Engine.DAL.Repositories.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Engine.Tests.Repositories
{
	public class TestRepositoryBase: IDisposable
    {
        private readonly SqliteConnection _connection;

        public readonly IRepository Repository;

        private EngineContext _context;

        public EngineContext Context
		{ 
            get 
            {
                return _context;
            }
        }

		public TestRepositoryBase(RepositoryType type)
        {
            _connection = new SqliteTestConnection().Connection;
            _connection.Open();
            var options = new DbContextOptionsBuilder<EngineContext>()
                    .UseSqlite(_connection)
                    .Options;
            _context = new EngineContext(options);
            Repository = GetRepository(type, _context);
            _context.Database.EnsureCreated();
        }

        private IRepository GetRepository(RepositoryType type, EngineContext context) {
            switch (type) {
                case RepositoryType.Companies:
                    return new CompaniesRepository(context);
                case RepositoryType.Logs:
                    return new LogsRepository(context);
                default:
                    return new UsersRepository(context);
            }
        }

        public void Dispose()
        {
            Repository.Dispose();
            _connection.Close();
        }
    }

    public enum RepositoryType {
        Users,
        Companies,
        Logs
    }
}
