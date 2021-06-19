using Engine.Services.Companies;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Engine.DAL.Repositories.Companies;
using Engine.Tests.Repositories.Companies;

namespace Engine.Tests.Services.Companies
{
	public class CompaniesTestService: CompaniesService, IDisposable
	{
		private static ILogger<CompaniesService> GetLogger()
		{
			var loggerMock = new Mock<ILogger<CompaniesService>>();
			return loggerMock.Object;
		}

		private static ICompaniesRepository GetRepo()
		{
			return new CompaniesTestRepository();
		}

		public  CompaniesTestService(): base(GetRepo(), GetLogger()) {
		}

		public void Dispose()
		{
		}
	}
}
