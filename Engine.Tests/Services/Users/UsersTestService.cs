using Engine.DAL.Repositories.Users;
using Engine.Tests.Repositories.Users;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Engine.Services.Users;

namespace Engine.Tests.Services.Users
{
	public class UsersTestService: UsersService, IDisposable
	{

		private static ILogger<UsersService> GetLogger()
		{
			var loggerMock = new Mock<ILogger<UsersService>>();
			return loggerMock.Object;
		}

		private static IUsersRepository GetRepo()
		{
			return new UsersTestRepository();
		}

		public  UsersTestService(): base(GetRepo(), GetLogger()) {
			
		}

		public void Dispose()
		{
		}
	}
}
