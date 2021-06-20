using Microsoft.Extensions.Logging;
using Moq;
using System;
using Engine.DAL.Repositories.Companies;
using Engine.Tests.Repositories.Companies;
using Engine.Services.WorkingServices.Companies;

namespace Engine.Services.WorkingServices.Companies
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
