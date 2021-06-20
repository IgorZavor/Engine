using Engine.Cache;
using Engine.CacheModels;
using Engine.DAL.Repositories.Logs;
using Engine.Models.In;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Engine.Services;
using System.Linq;
using Engine.Models.Out;
using Engine.DAL.Models;
using Engine.Services.Resolvers;
using Engine.Services.WorkingServices;
using Engine.Services.LogsServices;

namespace Engine.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/xml")]
	public class EngineController : ControllerBase
	{
		private readonly ILogger<EngineController> _logger;
		private IMemoryCache _cache;

		public EngineController(ILogger<EngineController> logger, IMemoryCache cache)
		{
			_logger = logger;
			_cache = cache;
		}

		[HttpPost, Route("generate")]
		public async Task<ActionResult> Generate(GenerateModel model, [FromServices] WorkingServiceResolver resolver)
		{
			if (model.Count <= 0) 
			{
				return new JsonResult(new ResultOut { Message = "Generation count must be more than 0", Error = false });
			}
			var start = DateTime.Now;
			IWorkingService repositoryService = null;
			try
			{
				var table = (Tables)Enum.Parse(typeof(Tables), model.Table, true);
				var t = typeof(User);
				repositoryService = resolver(table);
				await repositoryService.Generate(model.Count);
				var end = DateTime.Now;
				return new JsonResult(new ResultOut { Message = $"Generation has done successfully. Generation time is {end - start}", Error = false });
			}
			catch (ArgumentException ex)
			{
				var message = $"Generation has been failed.  {model.Table} table is not exist. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new ResultOut { Message = message, Error = true });
			}
			catch (Exception ex)
			{
				var message = "Generation has been failed. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new ResultOut { Message = message, Error = true });
			}
			finally 
			{
				if (repositoryService != null)
				{
					repositoryService.Dispose();
				}
			}
		}

		[HttpDelete, Route("clear")]
		public async Task<ActionResult> Clear(ClearModel model, [FromServices] ServiceResolver resolver)
		{
			var start = DateTime.Now;
			IBaseService repositoryService = null;
			try
			{
				var table = (Tables)Enum.Parse(typeof(Tables), model.Table, true);
				repositoryService = resolver(table);
				await repositoryService.Clear();
				var end = DateTime.Now;
				return new JsonResult(new ResultOut { Message = $"Cleaning has done successfully. Cleaning time is {end - start}", Error = false });
			}
			catch (ArgumentException ex)
			{
				var message = $"Generation has been failed.  {model.Table} table is not exist. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new ResultOut { Message = message, Error = true });
			}
			catch (Exception ex)
			{
				var message = "Cleaning has been failed. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult (new ResultOut { Message = message, Error = true });
			}
			finally 
			{
				if (repositoryService != null)
				{
					repositoryService.Dispose();
				}
			}
		}

		[HttpPost, Route("filterAndSum")]
		public async Task<ActionResult> FilterAndSum(FilterByModel model, [FromServices] ServiceResolver serviceResolver) 
		{
			var sum = 0;
			IBaseService repositoryService = null;
			try
			{
				var table = (Tables)Enum.Parse(typeof(Tables), model.Table, true);
				repositoryService = serviceResolver(table);
				sum = await repositoryService.FilterBy(model.FilterColumn, model.SummaryColumn, model.Filters.Select(f => f.Value).ToList());

				WriteToCache(CacheKeys.FilterBy, sum, model.Author, GetFilter(model.FilterColumn, model.Filters));
				return new JsonResult (new SumOut { Message = "Calculation success.", Error = false, Sum = sum }) ;
			}
			catch (ArgumentException ex)
			{
				var message = "Calculation is failed. Table or column is not exist. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new SumOut { Message = message, Error = true, Sum = -1 });
			}
			catch (Exception ex)
			{
				var message = "Calculation is failed. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new SumOut { Message = message, Error = true, Sum = -1 });
			}
			finally
			{
				if (repositoryService != null) 
				{
					repositoryService.Dispose();
				}
			}
		}

		[HttpPost, Route("writeSumToLogs")]
		public async Task<ActionResult> WriteSumToLogs([FromServices] IServiceScopeFactory serviceScopeFactory)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
			   var logsService = scope.ServiceProvider.GetRequiredService<ILogsService>();
				try
				{
					_logger.LogInformation("Getting value from cache...");
					var cacheLog = TryGetValueFromCache(CacheKeys.FilterBy);
					if (cacheLog != null)
					{
						await logsService.Write(cacheLog);
					}
					return new JsonResult(new ResultOut { Message = "A new log has been added successfully", Error = false });
				}
				catch (Exception ex)
				{
					var msg = "Writing to log is failed. Exception: " + ex;
					_logger.LogError(msg);
					return new JsonResult(new ResultOut { Message = msg, Error = true });
				}
				finally
				{
					logsService.Dispose();
				}
			}
		}

		[HttpGet, Route("getAllEntities")]
		public async Task<ActionResult> GetAllEntities(GetEntitiesModel model, [FromServices] ServiceResolver serviceResolver) 
		{
			IBaseService repositoryService = null;
			try
			{
				var table = (Tables)Enum.Parse(typeof(Tables), model.Table, true);
				repositoryService = serviceResolver(table);
				var entities = await repositoryService.GetEntities();
				return new JsonResult(new GetEntititesOut { Message = "Success", Error = false, Entities = entities });
			}
			catch (ArgumentException ex)
			{
				var message = $"Generation has been failed.  {model.Table} table is not exist. Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult(new ResultOut { Message = message, Error = true });
			}
			catch (Exception ex)
			{
				var message = "Exception: " + ex;
				_logger.LogError(message);
				return new JsonResult (new GetEntititesOut { Message = message, Error = true, Entities = null });
			}
			finally
			{
				if (repositoryService != null)
				{
					repositoryService.Dispose();
				}
			}
		}

		protected string GetFilter(string column, List<Filter> filters)
		{
			var filter = "where ";
			for (var i = 0; i < filters.Count; i++)
			{
				filter += $"{column}={filters[i].Value} ";
				if (i != filters.Count - 1)
				{
					filter += "|| ";
				}
			}
			return filter;
		}

		private void WriteToCache(string key, int sum, string author, string filter) 
		{
			var model = new CacheModel() {
				Author = author,
				Filter = filter,
				Sum = sum,
				DateTime = DateTime.Now
			};
			_cache.Set(key, model);
			_logger.LogInformation($"Sum has been saved in cache.");
		}

		protected CacheModel TryGetValueFromCache(string key) 
		{
			CacheModel cacheValue;
			if (!_cache.TryGetValue(key, out cacheValue))
			{
				_logger.LogInformation($"There is no cache with {key} key ");
			}
			else 
			{
				_logger.LogInformation($"Value with {key} key received from cache.");
			}
			return cacheValue;
		}
	}
}
