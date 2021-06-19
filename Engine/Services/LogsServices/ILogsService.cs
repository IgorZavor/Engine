
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.Services.LogsServices
{
	public interface ILogsService: IDisposable
	{
		Task Clear();
		IQueryable<object> GetEntities();
		Task<int> FilterBy(string filterColumn, string sumColumn, List<string> filterValues);
	}
}
