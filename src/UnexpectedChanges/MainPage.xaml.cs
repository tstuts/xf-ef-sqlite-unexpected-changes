using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnexpectedChanges.Data;
using Xamarin.Forms;

namespace UnexpectedChanges
{
	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();

			RunTest();
		}

		private void RunTest()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("Initializing");

			SQLitePCL.Batteries_V2.Init();

			string fileName = ":memory:";

			stringBuilder.AppendLine($"SQLite Filename = \"{fileName}\"");

			using(SqliteConnection connection = new SqliteConnection($"Filename={fileName}"))
			{
				connection.Open();

				using(ModelContext ctx = new ModelContext(connection))
				{
					ctx.Database.EnsureCreated();

					ctx.Add(new User { Id = Guid.NewGuid() });
					ctx.SaveChanges();
				}

				using(ModelContext ctx = new ModelContext(connection))
				{
					stringBuilder.AppendLine($"Pre-query HasChanges() = {ctx.ChangeTracker.HasChanges()}");

					List<User> users = ctx.Set<User>().ToList();

					stringBuilder.AppendLine($"Query result count: {users.Count}");

					User user = ctx.Set<User>().FirstOrDefault();

					stringBuilder.AppendLine($"Post-query HasChanges() = {ctx.ChangeTracker.HasChanges()}");

					EntityEntry entry = ctx.Entry(user);

					List<PropertyEntry> modifiedProperties = entry.Properties.Where(p => p.IsModified).ToList();

					if(modifiedProperties.Count == 0)
					{
						stringBuilder.AppendLine("No modified properties found");
					}
					else
					{
						foreach(PropertyEntry prop in modifiedProperties)
						{
							stringBuilder.AppendLine($"Modified property \"{prop.Metadata.Name}\"");
							stringBuilder.AppendLine($"Original: {prop.OriginalValue}");
							stringBuilder.AppendLine($" Current: {prop.CurrentValue}");
						}
					}
				}
			}

			this.OutputLabel.Text = stringBuilder.ToString();
		}
	}
}
