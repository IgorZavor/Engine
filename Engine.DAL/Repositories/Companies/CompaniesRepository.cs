using Engine.DAL.Contexts;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Engine.DAL.Models;

namespace Engine.DAL.Repositories.Companies
{
	public class CompaniesRepository : ICompaniesRepository
	{
		protected readonly EngineContext _context;
		private bool _disposed = false;

		public bool AutoDetectChangesEnabled
		{
			get => _context.ChangeTracker.AutoDetectChangesEnabled;
			set => _context.ChangeTracker.AutoDetectChangesEnabled = value;
		}

		public CompaniesRepository(EngineContext context)
		{
			_context = context;
		}

		public void Delete(int id)
		{
			var company = _context.Companies.Find(id);
			_context.Companies.Remove(company);
		}

		public async Task DeleteRow(int rowNum)
		{
			var company = await _context.Companies.Select(u => u).Skip(rowNum).Take(1).FirstAsync();
			_context.Companies.Remove(company);
		}

		public void DeletesRange(IEnumerable<object> entities) 
		{
			try
			{
				var companies = entities.Cast<Company>();
				_context.Companies.RemoveRange(companies);
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		public void Insert(object entity)
		{
			var company = (Company)entity;
			_context.Companies.Add(company);
		}

		public async Task<List<object>> FilterBy(string column, List<string> valuesForFilter)
		{
			if (valuesForFilter == null || valuesForFilter.Count == 0)
			{
				return await Task.FromResult(new List<object>());
			}
			var valuesToLower = valuesForFilter.Select(v => v.ToLower()).ToList();
			var enumValue = (Column)Enum.Parse(typeof(Column), column, true);
			switch (enumValue)
			{
				case Column.Country:
					return await FilterByCountry(valuesToLower);
				case Column.DateFounded:
					return await FilterByDatesFounded(valuesToLower);
				case Column.NumberOfEmployees:
					return await FilterByNumberOfEmployees(valuesToLower);
				case Column.Name:
					return await FilterByNames(valuesToLower);
				default:
					throw new InvalidOperationException($"Impossible filter table by {column} column");
			}
		}

		public async Task<List<object>> FilterByNames(List<string> names)
		{
			return await _context.Companies.Where(u => names.Contains(u.Name.ToLower())).ToListAsync<object>();
		}

		public async Task<List<object>> FilterByDatesFounded(List<string> dates)
		{
			var datesInt = new List<int>();
			dates.ForEach(d => datesInt.Add(int.Parse(d)));
			return await _context.Companies.Where(u => datesInt.Contains(u.YearFounded)).ToListAsync<object>();
		}

		public async Task<List<object>> FilterByNumberOfEmployees(List<string> numbersOfEmployees)
		{
			var numbersOfEmployeesInt = new List<int>();
			numbersOfEmployees.ForEach(d => numbersOfEmployeesInt.Add(int.Parse(d)));
			return await _context.Companies.Where(u => numbersOfEmployeesInt.Contains(u.YearFounded)).ToListAsync<object>();
		}

		public async Task<List<object>> FilterByCountry(List<string> countries)
		{
			return await _context.Companies.Where(u => countries.Contains(u.Country.ToLower())).ToListAsync<object>();
		}


		public void InsertRange(IEnumerable<object> entities)
		{
			try 
			{
				var companies = entities.Cast<Company>();
				_context.Companies.AddRange(companies);
			}
			catch (InvalidCastException e) 
			{
				throw e;
			}
		}

		public async Task<object> Get(int id)
		{
			return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
		}

		public IQueryable<object> GetEntities() {
			return _context.Companies.AsQueryable<Company>();
		}

		public async Task<int> SaveAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task<int> GetCountAsync()
		{
			return await _context.Companies.CountAsync();
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
