using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.DAL.Repositories
{
	public interface IRepository: IDisposable
	{
		void Insert(object entity);
		void InsertRange(IEnumerable<object> entities);
		void Delete(int id);
		Task DeleteRow(int row);
		void DeletesRange(IEnumerable<object> entities);
		Task<object> GetEntity(int id);
		Task<List<object>> GetEntities();
		Task<int> SaveAsync();
		bool AutoDetectChangesEnabled { get; set; }
		Task<int> GetCountAsync();

		Task<List<object>> FilterBy(string column, List<string> valueForFilter);
	}
}
