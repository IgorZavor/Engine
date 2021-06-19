using Engine.Controllers;
using Engine.DAL.Repositories.Users;
using Engine.DAL.Repositories.Companies;
using Engine.DAL.Repositories.Logs;
using Engine.Tests.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Engine.Tests.Repositories.Users;
using Engine.DAL.Repositories;
using Engine.Tests.Repositories.Companies;

namespace Engine.Tests.Controllers
{
	public class EngineTestController : EngineController
	{
		private static IServiceScopeFactory _serviceScopeFactory;
		private static IServiceScope _scope;

		public IServiceScopeFactory ServiceScopeFactory
		{
			get
			{
				return _serviceScopeFactory;
			}
		}

		public IServiceScope Scope 
		{
			get 
			{
				return _scope;
			}
		}

		protected static IRepository repository;

		static EngineTestController()
		{
			SetUpServiceScopeFactory();
		}

		private static IMemoryCache GetMemoryCache() {
			var services = new ServiceCollection();
			services.AddMemoryCache();
			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider.GetService<IMemoryCache>();
		}

		public EngineTestController(): base(new Mock<ILogger<EngineController>>().Object, GetMemoryCache())
		{
		}


		private static void SetUpServiceScopeFactory()
		{
			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			SetUpServiceScope();
			serviceScopeFactory.Setup(s => s.CreateScope()).Returns(_scope);
			_serviceScopeFactory = serviceScopeFactory.Object;
		}

		private static IServiceProvider SetUpServiceProvider()
		{
			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(sp => sp.GetService(typeof(IUsersRepository))).Returns(repository = new UsersTestRepository());
			serviceProvider.Setup(sp => sp.GetService(typeof(ICompaniesRepository))).Returns(repository = new CompaniesTestRepository());
			serviceProvider.Setup(sp => sp.GetService(typeof(ILogsRepository))).Returns((new TestRepositoryBase(RepositoryType.Logs).Repository) as ILogsRepository);

			return serviceProvider.Object;
		}

		private static void SetUpServiceScope()
		{
			var scope = new Mock<IServiceScope>();
			scope.Setup(s => s.ServiceProvider).Returns(SetUpServiceProvider());
			_scope = scope.Object;
		}
	}
}
