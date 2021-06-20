using Engine.DAL.Models;
using Engine.DAL.Repositories.Logs;
using Microsoft.Extensions.Logging;
using System;

namespace Engine.Services.LogsServices.Logs
{
	public class LogsService: LogsServiceBase
	{
		public LogsService(ILogsRepository repository, ILogger<LogsService> logger) : base(repository, logger)
		{
		}

		protected override int GetValue(object entity, string column)
		{
			var summartColumn = (Columns)Enum.Parse(typeof(Columns), column, true);
			switch (summartColumn) {
				case Columns.Id:
					return ((Log)entity).Id;
				case Columns.Sum:
					return ((Log)entity).Sum;
				default: 
					throw new InvalidOperationException($"{TableName} table doens't contain the {column} colunm or summation is not supported by {column} column ");
			}
		}

		protected override string TableName => "Logs";
	}
}
