using System;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices
{
	public interface IWorkingService: IBaseService
	{
		Task Generate(int count);
	}
}
