using System.Linq;
using System.Threading.Tasks;

namespace Engine.Services.WorkingServices
{
	interface IWorkService
	{
		Task Generate(int count);
		Task Clear();
		IQueryable<object> GetEntities();
	}
}
