using Engine.DAL.Contexts;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Engine.DAL.Models;

namespace Engine.DAL.Repositories.Users
{
	public class UsersRepository : IUsersRepository
	{
		protected readonly EngineContext _context;
		private bool _disposed = false;

		public bool AutoDetectChangesEnabled
		{
			get => _context.ChangeTracker.AutoDetectChangesEnabled;
			set => _context.ChangeTracker.AutoDetectChangesEnabled = value;
		}

		public UsersRepository(EngineContext context)
		{
			_context = context;
		}

		public void Delete(int id)
		{
			var user = _context.Users.Find(id);
			_context.Users.Remove(user);
		}

		public async Task DeleteRow(int rowNum)
		{
			var user = await _context.Users.Skip(rowNum).Take(1).FirstAsync();
			_context.Users.Remove(user);
		}

		public void DeletesRange(IEnumerable<object> entities) 
		{
			try
			{
				var users = entities.Cast<Models.User>();
				_context.Users.RemoveRange(users);
			}
			catch (InvalidCastException e) {
				throw e;
			}
		}

		public void Insert(object entity)
		{
			var user = entity as Models.User;
			if (user != null)
			{
				_context.Users.Add(user);
			}
		}

		public async Task<List<object>> FilterBy(string column, List<string> valueForFilter) 
		{
			if (valueForFilter != null || valueForFilter.Count > 0)
			{
				var valuesToLower = valueForFilter.Select(v => v.ToLower()).ToList();
				var enumValue = (Columns)Enum.Parse(typeof(Columns), column, true);
				switch (enumValue)
				{
					case Columns.Id:
						return await FilterByIds(valuesToLower);
					case Columns.Country:
						return await FilterByCountry(valuesToLower);
					case Columns.Surname:
						return await FilterBySurnames(valuesToLower);
					case Columns.Age:
						return await FilterByAges(valuesToLower);
					case Columns.Name:
						return await FilterByNames(valuesToLower);
					default:
						throw new InvalidOperationException($"Impossible filter table by {column} column");
				}
			}
			else
			{
				return await Task.FromResult(new List<object>());
			}
		}

		private async Task<List<object>> FilterByIds(List<string> ids)
		{
			var idsInt = ids.Select(n => int.Parse(n));
			return await _context.Users.Where(u => idsInt.Contains(u.Id)).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByNames(List<string> names)
		{
			return await _context.Users.Where(u => names.Contains(u.Name.ToLower())).ToListAsync<object>();
		}

		private async Task<List<object>> FilterBySurnames(List<string> surnames)
		{
			return await _context.Users.Where(u => surnames.Contains(u.Surname.ToLower())).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByCountry(List<string> countries)
		{
			return await _context.Users.Where(u => countries.Contains(u.Country.ToLower())).ToListAsync<object>();
		}
		private async Task<List<object>> FilterByAges(List<string> ages)
		{
			var agesInt = ages.Select(n => int.Parse(n));
			return await _context.Users.Where(u => agesInt.Contains(u.Age)).ToListAsync<object>();
		}

		public void InsertRange(IEnumerable<object> entities)
		{
			try
			{
				var users = entities.Cast<Models.User>();
				_context.Users.AddRange(users);
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		public async Task<object> GetEntity(int id)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<List<object>> GetEntities() {
			return await _context.Users.AsQueryable<object>().ToListAsync();
		}

		public async Task<int> SaveAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task<int> GetCountAsync()
		{
			return await _context.Users.CountAsync();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
			}
			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	}
}
