using Engine.Services.WorkingServices;

namespace Engine.Services.Resolvers
{
	public delegate IWorkingService WorkingServiceResolver(Tables tableType);
}
