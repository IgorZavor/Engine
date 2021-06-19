using Engine.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Engine.DAL.Contexts
{
	public class EngineContext : DbContext
	{
		public EngineContext() { }
		public EngineContext(DbContextOptions<EngineContext> options): base(options) {}
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Log> Logs { get; set; }
		public virtual DbSet<Company> Companies { get; set; }
	}
}
