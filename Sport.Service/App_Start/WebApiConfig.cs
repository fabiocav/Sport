﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Sport;
using Sport.Service.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security.Providers;
using System.Data.Entity.Migrations;

namespace Sport.Service
{
    public static class WebApiConfig
    {
		public static bool IsDemoMode
		{
			get;
			set;
		}

		public static int MaxLeagueMembershipCount
		{
			get;
			set;
		}

		public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
			//options.LoginProviders.Remove(typeof(GoogleLoginAuthenticationProvider));
			//options.LoginProviders.Add(typeof(GoogleLoginAuthenticationProvider));

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

			bool isDemoMode;
			var boolString = System.Configuration.ConfigurationManager.AppSettings["IsDemoMode"];
			bool.TryParse(boolString, out isDemoMode);
			IsDemoMode = isDemoMode;

			int maxCount;
			var intString = System.Configuration.ConfigurationManager.AppSettings["MaxLeagueMembershipCount"];
			int.TryParse(intString, out maxCount);
			MaxLeagueMembershipCount = maxCount;

			//config.SetIsHosted(true);
			// To display errors in the browser during development, uncomment the following
			// line. Comment it out again when you deploy your service for production use.
			// config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

			//Database.SetInitializer(new SportRanker_MatchOnInitializer());
			//var migrator = new DbMigrator(new Configuration
			//{
			//	AutomaticMigrationDataLossAllowed = true,
			//	AutomaticMigrationsEnabled = true
			//});
			//migrator.Update();
		}
	}

	public class SportInitializer : ClearDatabaseSchemaIfModelChanges<AppDataContext>
    {
        protected override void Seed(AppDataContext context)
        {
            base.Seed(context);
        }
    }
}

