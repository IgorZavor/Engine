using Engine.CacheModels;
using Engine.DAL.Models;
using Engine.DAL.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Services.LogsServices
{
	public abstract class LogsServiceBase: ILogsService
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

		public LogsServiceBase(IRepository repository, ILogger logger) {
			_repository = repository;
			_logger = logger;
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

		public async Task<List<object>> GetEntities() {
			try
			{
				var entites = await _repository.GetEntities();
				_logger.LogInformation($"Getting entites is done");
				return entites;
			}
			catch (Exception ex)
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
				var results = await _repository.FilterBy(filterColumn, filterValues);
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
			finally
			{
			}
			return sum;
		}

		protected abstract int GetValue(object entity, string column);

		public void Dispose()
		{
			_repository.Dispose();
		}

		public async Task Write(CacheModel cache)
		{
			var log = new Log()
			{
				Author = cache.Author,
				DateTime = cache.DateTime.ToString("MM/dd/yy H:mm:ss zzz"),
				Filter = cache.Filter,
				Sum = cache.Sum
			};
			_repository.Insert(log);
			await _repository.SaveAsync();
		}

		protected virtual string TableName => "";
	}
}
