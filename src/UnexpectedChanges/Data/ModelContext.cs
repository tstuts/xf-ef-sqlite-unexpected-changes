using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnexpectedChanges.Data
{
	internal sealed class ModelContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		private readonly SqliteConnection _sqliteConnection;

		public ModelContext(SqliteConnection sqliteConnection)
		{
			_sqliteConnection = sqliteConnection;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseSqlite(_sqliteConnection);
		}
	}
}
