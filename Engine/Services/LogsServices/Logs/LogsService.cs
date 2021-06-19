using Engine.DAL.Models;
using Engine.DAL.Repositories.Logs;
using Microsoft.Extensions.Logging;
using System;

namespace Engine.Services.LogsServices.Logs
{
	public class LogsService: LogsServiceBase
	{
		public LogsService(ILogsRepository repository, ILogger logger) : base(repository, logger)
		{
		}

		protected override int GetValue(object entity, string column)
		{
			var summartColumn = (SummaryColumns)Enum.Parse(typeof(SummaryColumns), column, true);
			switch (summartColumn) {
				case SummaryColumns.Id:
					return ((Log)entity).Id;
				default: 
					throw new InvalidOperationException($"Users table doens't contain the {column} colunm or summation is not supported by {column} column ");
			}
		}

		protected override string TableName => "Logs";
	}
}
