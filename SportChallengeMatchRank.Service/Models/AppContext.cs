using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using SportChallengeMatchRank;
using System.Collections.Generic;
using System;
using Microsoft.WindowsAzure.Mobile.Service.Notifications;

namespace SportChallengeMatchRank.Service.Models
{
	public class AppDataContext : DbContext
	{
		// To enable Entity Framework migrations in the cloud, please ensure that the 
		// service name, set by the 'MS_MobileServiceName' AppSettings in the local 
		// Web.config, is the same as the service name when hosted in Azure.
		private const string connectionStringName = "Name=MS_TableConnectionString";

		public AppDataContext() : base(connectionStringName)
		{
		}

		public DbSet<Athlete> Athletes
		{
			get;
			set;
		}

		public DbSet<Membership> Memberships
		{
			get;
			set;
		}

		public DbSet<League> Leagues
		{
			get;
			set;
		}

		public DbSet<Challenge> Challenges
		{
			get;
			set;
		}

		public DbSet<GameResult> GameResults
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
			modelBuilder.Entity<Membership>().ToTable("Membership");
			modelBuilder.Entity<Challenge>().ToTable("Challenge");
			modelBuilder.Entity<GameResult>().ToTable("GameResult");

			modelBuilder.Entity<GameResult>().HasOptional(a => a.Challenge)
				.WithMany().HasForeignKey(a => a.ChallengeId);

			modelBuilder.Entity<Challenge>().HasOptional(a => a.ChallengeeAthlete)
				.WithMany().HasForeignKey(a => a.ChallengeeAthleteId);

			modelBuilder.Entity<Challenge>().HasOptional(a => a.ChallengerAthlete)
				.WithMany().HasForeignKey(a => a.ChallengerAthleteId);

			modelBuilder.Entity<Challenge>().HasOptional(a => a.League)
				.WithMany().HasForeignKey(a => a.LeagueId);

			modelBuilder.Entity<League>().HasOptional(a => a.CreatedByAthlete)
				.WithMany().HasForeignKey(a => a.CreatedByAthleteId);

			modelBuilder.Entity<Membership>().HasRequired(m => m.League)
				.WithMany(l => l.Memberships)
				.HasForeignKey(m => m.LeagueId);

			modelBuilder.Entity<Membership>().HasRequired(m => m.Athlete)
				.WithMany(a => a.Memberships)
				.HasForeignKey(m => m.AthleteId);

			modelBuilder.Conventions.Add(
				new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
					"ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
		}

		public static IPushMessage GetPush(Athlete athlete, string message, Dictionary<string, object> payload)
		{
			switch(athlete.DevicePlatform)
			{
				case "iOS":

					var apn = new ApplePushMessage(message, TimeSpan.FromHours(1));

					foreach(var kvp in payload)
						apn.Add(kvp.Key, kvp.Value);

					return apn;
				case "Android":

					var dict = new Dictionary<string, string>();
					dict.Add("message", message);

					foreach(var kvp in payload)
						dict.Add(kvp.Key, kvp.Value.ToString());

					return new GooglePushMessage(dict, TimeSpan.FromHours(1));
			}

			return null;
		}
	}
}