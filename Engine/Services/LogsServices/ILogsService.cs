
using Engine.CacheModels;
using System;
using System.Threading.Tasks;

namespace Engine.Services.LogsServices
{
	public interface ILogsService: IBaseService
	{
		Task Write(CacheModel cache);
	}
}
