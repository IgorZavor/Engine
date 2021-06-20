using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.Services
{
	public interface IBaseService: IDisposable
	{
		Task Clear();
		Task<List<object>> GetEntities();
		Task<int> FilterBy(string filterColumn, string sumColumn, List<string> filterValues);
	}
}
