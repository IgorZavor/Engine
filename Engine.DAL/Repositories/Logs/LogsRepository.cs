using Engine.DAL.Contexts;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Engine.DAL.Models;
using System.Linq;
using System.Collections.Generic;

namespace Engine.DAL.Repositories.Logs
{
	public class LogsRepository : ILogsRepository
	{
		protected readonly EngineContext _context;
		private bool _disposed = false;

		public bool AutoDetectChangesEnabled 
		{ 
			get => _context.ChangeTracker.AutoDetectChangesEnabled; 
			set => _context.ChangeTracker.AutoDetectChangesEnabled = value; 
		}

		public LogsRepository(EngineContext context)
		{
			_context = context;
		}

		public void Insert(object entity)
		{
			var log = entity as Log;
			if (log != null)
			{
				_context.Logs.Add(log);
			}
		}

		public async Task<int> SaveAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task DeleteRow(int row)
		{
			var log = await _context.Logs.Select(u => u).Skip(row).Take(1).FirstAsync();
			_context.Logs.Remove(log);
		}

		public async Task<List<object>> GetEntities()
		{
			return await _context.Logs.AsQueryable<object>().ToListAsync();
		}

		public void InsertRange(IEnumerable<object> entities)
		{
			try
			{
				var logs = entities.Cast<Log>();
				_context.Logs.AddRange(logs);
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		public void Delete(int id)
		{
			var log = _context.Logs.Find(id);
			_context.Logs.Remove(log);
		}

		public void DeletesRange(IEnumerable<object> entites)
		{
			try
			{
				var logs = entites.Cast<Log>();
				_context.Logs.RemoveRange(logs);
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		public async Task<object> GetEntity(int id)
		{
			return await _context.Logs.FirstOrDefaultAsync(l => l.Id == id);
		}

		public async Task<int> GetCountAsync()
		{
			return await _context.Logs.CountAsync();
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
						return await FilterByIDs(valuesToLower);
					case Columns.Author:
						return await FilterByAuthors(valuesToLower);
					case Columns.DateTime:
						return await FilterByDateTimes(valuesToLower);
					case Columns.Filter:
						return await FilterByFilters(valuesToLower);
					case Columns.Sum:
						return await FilterBySums(valuesToLower);
					default:
						throw new InvalidOperationException($"Impossible filter table by {column} column");
				}
			}
			else
			{
				return await Task.FromResult(new List<object>());
			}
		}

		private async Task<List<object>> FilterBySums(List<string> sums)
		{
			var sumsInt = sums.Select(n => int.Parse(n));
			return await _context.Logs.Where(u => sumsInt.Contains(u.Sum)).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByIDs(List<string> ids)
		{
			var idsInt = ids.Select(n => int.Parse(n));
			return await _context.Logs.Where(u => idsInt.Contains(u.Id)).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByAuthors(List<string> authors)
		{
			return await _context.Logs.Where(u => authors.Contains(u.Author.ToLower())).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByDateTimes(List<string> dateTimes)
		{
			return await _context.Logs.Where(u => dateTimes.Contains(u.DateTime.ToLower())).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByFilters(List<string> filters)
		{
			return await _context.Logs.Where(u => filters.Contains(u.Filter.ToLower())).ToListAsync<object>();
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
