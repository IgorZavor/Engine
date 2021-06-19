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
		public async Task<ActionResult> Generate(GenerateIn model, [FromServices] WorkingServiceResolver resolver)
		{
			if (model.Count <= 0) 
			{
				return new JsonResult(new ResultOut { Message = "Generation count must be more 0", Error = false });
			}
			var start = DateTime.Now;
			IWorkingService repositoryService = null;
			try
			{
				var table = (Enums.Tables)Enum.Parse(typeof(Enums.Tables), model.Table, true);
				var t = typeof(User);
				repositoryService = resolver(table);
				await repositoryService.Generate(model.Count);
				var end = DateTime.Now;
				return new JsonResult(new ResultOut { Message = $"Generation has done successfully. Generation time is {end - start}", Error = false });
			}
			catch (Exception ex)
			{
				return new JsonResult(new ResultOut { Message = "Generation has been failed. " + "Exception: " + ex, Error = true });
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
		public async Task<ActionResult> Clear(ClearIn model, [FromServices] WorkingServiceResolver resolver)
		{
			var start = DateTime.Now;
			IWorkingService repositoryService = null;
			try
			{
				var table = (Enums.Tables)Enum.Parse(typeof(Enums.Tables), model.Table, true);
				repositoryService = resolver(table);
				await repositoryService.Clear();
				var end = DateTime.Now;
				return new JsonResult(new ResultOut { Message = $"Cleaning has done successfully. Cleaning time is {end - start}", Error = false });
			}
			catch (Exception ex)
			{
				_logger.LogError("Exception: " + ex);
				return new JsonResult (new ResultOut { Message = "Cleaning has been failed." + "Exception: " + ex, Error = true });
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
		public async Task<ActionResult> FilterAndSum(FilterByIn model, [FromServices] ServiceResolver serviceResolver) 
		{
			var sum = 0;
			IBaseService repositoryService = null;
			try
			{
				var table = (Enums.Tables)Enum.Parse(typeof(Enums.Tables), model.Table, true);
				repositoryService = serviceResolver(table);
				sum = await repositoryService.FilterBy(model.FilterColumn, model.SummaryColumn, model.Filters.Select(f => f.Value).ToList());

				WriteToCache(CacheKeys.FilterBy, sum, model.Author, GetFilter(model.FilterColumn, model.Filters));
				return new JsonResult (new SumOut { Message = "Calculation success", Error = false, Sum = sum }) ;
			}
			catch (Exception ex)
			{
				_logger.LogError("Exception: " + ex);
				return new JsonResult(new SumOut { Message = "Calculation is failed." + "Exception: " + ex, Error = true, Sum = -1 });
			}
			finally
			{
				repositoryService.Dispose();
			}
		}

		[HttpPost, Route("writeSumToLogs")]
		public async Task<ActionResult> WriteSumToLogs([FromServices] IServiceScopeFactory serviceScopeFactory)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
			   var logsRepository = scope.ServiceProvider.GetRequiredService<ILogsRepository>();
				try
				{
					_logger.LogInformation("Getting value from cache...");
					var memoryLog = TryGetValueFromCache(CacheKeys.FilterBy);
					var log = new DAL.Models.Log() { Author = memoryLog.Author, DateTime = memoryLog.DateTime.ToString("MM/dd/yy H:mm:ss zzz") };
					logsRepository.Insert(log);
					await logsRepository.SaveAsync();
					return new JsonResult(new ResultOut { Message = "A new log has been added successfully", Error = false });
				}
				catch (Exception ex)
				{
					_logger.LogError("Exception: " + ex);
					return new JsonResult(new ResultOut { Message = "Writing to log is failed." + "Exception: " + ex, Error = true });
				}
				finally
				{
					logsRepository.Dispose();
				}
			}
		}

		[HttpGet, Route("getAllEntities")]
		public async Task<ActionResult> GetAll(GetEntitiesIn model, [FromServices] ServiceResolver serviceResolver) 
		{
			IBaseService repositoryService = null;
			try
			{
				var table = (Enums.Tables)Enum.Parse(typeof(Enums.Tables), model.Table, true);
				repositoryService = serviceResolver(table);
				var entities = repositoryService.GetEntities().ToList();
				return new JsonResult(new GetEntititesOut { Message = "Success", Error = false, Entities = entities });
			}
			catch (Exception ex)
			{
				_logger.LogError("Exception: " + ex);
				return new JsonResult (new GetEntititesOut { Message = "Exception: " + ex, Error = true, Entities = null });
			}
			finally
			{
				repositoryService.Dispose();
			}
		}

		protected string GetFilter(string column, List<Filter> filters)
		{
			var filter = "where ";
			for (var i = 0; i < filters.Count; i++)
			{
				if (i == filters.Count - 1)
				{
					filter += $"{column}={filters[i].Value} ||";
				}
				else
				{
					filter += $"{column}={filters[i].Value}";
				}
			}
			return filter;
		}

		private void WriteToCache(string key, int result, string author, string filter) 
		{
			var model = new CacheModel() {
				Author = author,
				ColumnFilter = filter,
				Result = result,
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
				_logger.LogInformation($"There is no value for {key} key in cache.");
			}
			else 
			{
				_logger.LogInformation($"Value with {key} key received from cache.");
			}
			return cacheValue;
		}
	}
}
