using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using SportRankerMatchOn;

namespace SportRankerMatchOn.Service.Models
{
	public class SportRankerMatchOnContext : DbContext
	{
		// To enable Entity Framework migrations in the cloud, please ensure that the 
		// service name, set by the 'MS_MobileServiceName' AppSettings in the local 
		// Web.config, is the same as the service name when hosted in Azure.
		private const string connectionStringName = "Name=MS_TableConnectionString";

		public SportRankerMatchOnContext() : base(connectionStringName)
		{
		}

		public DbSet<Athlete> Athletes
		{
			get;
			set;
		}

		public DbSet<Member> Members
		{
			get;
			set;
		}

		public DbSet<League> Leagues
		{
			get;
			set;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			string schema = ServiceSettingsDictionary.GetSchemaName();
			if (!string.IsNullOrEmpty(schema))
			{
				modelBuilder.HasDefaultSchema(schema);
			}

			modelBuilder.Entity<Athlete>().ToTable("Athlete");
			modelBuilder.Entity<League>().ToTable("League");
			modelBuilder.Entity<Member>().ToTable("Member");

			modelBuilder.Entity<Member>().HasRequired(m => m.League)
				.WithMany(l => l.Members)
				.HasForeignKey(m => m.LeagueId);

			modelBuilder.Entity<Member>().HasRequired(m => m.Athlete)
				.WithMany(a => a.LeagueAssociations)
				.HasForeignKey(m => m.AthleteId);

			modelBuilder.Conventions.Add(
				new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
					"ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
		}
	}
}