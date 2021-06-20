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
			var enumValue = (Columns)Enum.Parse(typeof(Columns), column, true);
			switch (enumValue)
			{
				case Columns.Id:
					return await FilterByIds(valuesToLower);
				case Columns.Country:
					return await FilterByCountry(valuesToLower);
				case Columns.YearFounded:
					return await FilterByDatesFounded(valuesToLower);
				case Columns.NumberOfEmployees:
					return await FilterByNumberOfEmployees(valuesToLower);
				case Columns.Name:
					return await FilterByNames(valuesToLower);
				default:
					throw new InvalidOperationException($"Impossible filter table by {column} column");
			}
		}

		private async Task<List<object>> FilterByIds(List<string> ids)
		{
			var idsInt = ids.Select(id => int.Parse(id));
			return await _context.Companies.Where(u => idsInt.Contains(u.Id)).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByNames(List<string> names)
		{
			return await _context.Companies.Where(u => names.Contains(u.Name.ToLower())).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByDatesFounded(List<string> dates)
		{
			var datesInt = dates.Select(n => int.Parse(n));
			return await _context.Companies.Where(u => datesInt.Contains(u.YearFounded)).ToListAsync<object>();
		}

		private async Task<List<object>> FilterByNumberOfEmployees(List<string> numbersOfEmployees)
		{
			var numbersOfEmployeesInt = numbersOfEmployees.Select(n => int.Parse(n));
			return await _context.Companies.Where(u => numbersOfEmployeesInt.Contains(u.NumberOfEmployees)).ToListAsync<object>();
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

		public async Task<object> GetEntity(int id)
		{
			return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<List<object>> GetEntities() {
			return await _context.Companies.AsQueryable<object>().ToListAsync();
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
