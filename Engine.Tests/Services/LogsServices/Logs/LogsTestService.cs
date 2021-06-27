using Microsoft.Extensions.Logging;
using Moq;
using System;
using Engine.Tests.Repositories.Logs;
using Engine.DAL.Repositories.Logs;

namespace Engine.Services.LogsServices.Logs
{
	public class LogsTestService: LogsService, IDisposable
	{
		private static ILogger<LogsService> GetLogger()
		{
			var loggerMock = new Mock<ILogger<LogsService>>();
			return loggerMock.Object;
		}

		private static ILogsRepository GetRepo()
		{
			return new LogsTestRepository();
		}

		public LogsTestService(): base(GetRepo(), GetLogger()) {
		}

		public void Dispose()
		{
		}

		public void Dispose(bool disposeBase = false)
		{
			if (disposeBase)
			{
				base.Dispose();
			}
		}
	}
}
