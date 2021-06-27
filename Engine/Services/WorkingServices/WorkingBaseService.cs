using Engine.DAL.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices
{
	public abstract class WorkingBaseService : IWorkingService
	{
		private IRepository _repository;
		protected ILogger _logger;

		public IRepository Repository
		{
			get 
			{
				return _repository;
			}
		}

		public WorkingBaseService(IRepository repository, ILogger logger) {
			_repository = repository;
			_logger = logger;
		}

		public async Task Generate(int count)
		{
			var mtx = new Mutex();
			try
			{
				_repository.AutoDetectChangesEnabled = false;
				Parallel.For(0, count, (int userNum) =>
				{
					mtx.WaitOne();
					var insertModel = CreateEntity();
					_repository.Insert(insertModel);
					mtx.ReleaseMutex();
					_logger.LogInformation($"New model has been added.");
					Thread.Sleep(100);
				});
				await _repository.SaveAsync();
				_logger.LogInformation($"Saving has been done.");
			}
			catch (DataException ex)
			{
				throw ex;
			}
			finally
			{
				_repository.AutoDetectChangesEnabled = true;
			}
		}

		public async Task Clear()
		{
			var mtx = new Mutex();
			try
			{
				var rowsCount = await _repository.GetCountAsync();
				_repository.AutoDetectChangesEnabled = false;
				_logger.LogInformation($"Start clearing ${TableName}");
				Parallel.For(0, rowsCount, (int rowNum) =>
				{
					mtx.WaitOne();
					_repository.DeleteRow(rowNum);
					_logger.LogInformation($"Deleting row {rowNum}");
					mtx.ReleaseMutex();
					Thread.Sleep(100);
				});
				await _repository.SaveAsync();
				_logger.LogInformation($"clearing ${TableName} is done!");
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				_repository.AutoDetectChangesEnabled = true;
			}
		}

		public async Task<List<object>> GetEntities() 
		{
			try
			{
				var entites = await _repository.GetEntities();
				_logger.LogInformation($"Getting entites has been done");
				return entites;
			}
			catch (ArgumentNullException ex)
			{
				throw ex;
			}
		}

		public async Task<int> FilterBy(string filterColumn, string sumColumn, List<string> filterValues)
		{
			var mtx = new Mutex();
			var sum = 0;
			try
			{
				List<object> results = null;
				if (filterValues.Count == 0)
				{
					results = await GetEntities();
				}
				else 
				{
					results = await _repository.FilterBy(filterColumn, filterValues);
				}
				Parallel.For(0, results.Count, (int num) =>
				{
					mtx.WaitOne();
					sum += GetValue(results[num], sumColumn);
					mtx.ReleaseMutex();
					Thread.Sleep(100);
				});
				_logger.LogInformation($"{filterColumn} column has been summed up.");
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return sum;
		}

		protected abstract int GetValue(object entity, string column);

		protected abstract object CreateEntity();

		public void Dispose()
		{
			_repository.Dispose();
		}

		protected virtual string TableName => "";
	}
}
